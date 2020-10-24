#include"CaffParser.h"
#include"pch.h"
#include<string>
#include<iostream>
#include<fstream>
#include<stdint.h>
#include<list>

typedef struct caffHeader {
	char magic[5];
	uint64_t header_size;
	uint64_t num_anim;
}caffHeader;

typedef struct createDate {
	uint16_t year;
	uint8_t month;
	uint8_t day;
	uint8_t hour;
	uint8_t minute;
}createDate;

typedef struct ciffHeader {
	char magic[5];
	uint64_t header_size; //all fields included
	uint64_t content_size; //width*height*3 (RGB)
	uint64_t width; // can be 0
	uint64_t height; // can be 0
	std::string caption; // variable length special ending char: "\n"
	std::list<std::string> tags; //tags separated with \0
}ciffHeader;

typedef struct pixel {
	uint8_t r;
	uint8_t g;
	uint8_t b;
}pixel;

int ParseAndValidateCaff(std::string tempDir, std::string validCaffDir, std::string previewDir, std::string fileName) {

	//variables
	uint8_t id;
	uint64_t length;
	char char_buffer;
	bool generated = false;
	uint64_t duration;
	ciffHeader ciffH;
	caffHeader h;
	std::list<char> caption;
	int anims_read = 0;

	std::ifstream myfile;
	fileName += ".caff";
	std::string openPath = tempDir + fileName;
	std::ofstream image(previewDir + "preview_" + fileName.substr(0, fileName.find('.')) + ".ppm");
	myfile.open(openPath.c_str(), std::ios::in | std::ios::binary);

	if (myfile) {
		while (myfile.good() && !myfile.eof())
		{
#pragma region WhichBlock
			myfile.read(reinterpret_cast<char*>(&id), sizeof(uint8_t)); //read id byte
			if (id > 3) {
				break;
			}
			std::cout << "ID: " << unsigned(id) << std::endl;

			myfile.read(reinterpret_cast<char*>(&length), sizeof(uint64_t));
			std::cout << "Length: " << length << std::endl;
#pragma endregion

#pragma region ID1
			if (id == 0x1) {
				//myfile.read(reinterpret_cast<char*>(&buffer), length);
				myfile.read(reinterpret_cast<char*>(&h.magic), 4);
				myfile.read(reinterpret_cast<char*>(&h.header_size), 8);
				myfile.read(reinterpret_cast<char*>(&h.num_anim), 8);

				//restore balance in the force
				h.num_anim -= anims_read;

				//TODO append '\0' not as a dumbass
				h.magic[4] = '\0';

				std::cout << "Magic: " << h.magic << std::endl;
				std::cout << "Header size: " << h.header_size << std::endl;
				std::cout << "Num of anim: " << h.num_anim << std::endl;
			}
#pragma endregion

#pragma region ID2
			if (id == 0x2) {
				//id == 2 : CREDITS
				createDate date;
				uint64_t creator_len;
				myfile.read(reinterpret_cast<char*>(&date.year), 2);
				myfile.read(reinterpret_cast<char*>(&date.month), 1);
				myfile.read(reinterpret_cast<char*>(&date.day), 1);
				myfile.read(reinterpret_cast<char*>(&date.hour), 1);
				myfile.read(reinterpret_cast<char*>(&date.minute), 1);
				std::cout << "Created: " << date.year << "." << (int)date.month << "." << (int)date.day << ". " << (int)date.hour << ":" << (int)date.minute << std::endl;

				myfile.read(reinterpret_cast<char*>(&creator_len), sizeof(uint64_t));
				std::cout << "Creator len: " << creator_len << std::endl;

				std::string creator;
				for (int i = 0; i < creator_len; i++) {
					myfile.read(reinterpret_cast<char*>(&char_buffer), sizeof(char));
					creator += char_buffer;
				}
				
				std::cout << "Creator: " << creator << std::endl;
			}
#pragma endregion

#pragma region ID3
			if (id == 0x3 && h.num_anim != 0) {
				anims_read++;
				h.num_anim--;
				myfile.read(reinterpret_cast<char*>(&duration), sizeof(uint64_t));
				std::cout << "Duration: " << duration << std::endl;

				////////////////////////////////////////////////
				//                 CIFF READ                  //
				////////////////////////////////////////////////
				myfile.read(reinterpret_cast<char*>(&ciffH.magic), 4);

				//TODO append '\0' not as a dumbass
				ciffH.magic[4] = '\0';
				std::cout << "CIFF Magic: " << ciffH.magic << std::endl;

				myfile.read(reinterpret_cast<char*>(&ciffH.header_size), sizeof(uint64_t));
				std::cout << "CIFF Header size: " << ciffH.header_size << std::endl;

				myfile.read(reinterpret_cast<char*>(&ciffH.content_size), sizeof(uint64_t));
				std::cout << "CIFF Content size: " << ciffH.content_size << std::endl;


				myfile.read(reinterpret_cast<char*>(&ciffH.width), sizeof(uint64_t));
				std::cout << "CIFF Width: " << ciffH.width << std::endl;

				myfile.read(reinterpret_cast<char*>(&ciffH.height), sizeof(uint64_t));
				std::cout << "CIFF Height: " << ciffH.height << std::endl;

				caption.clear();

				//Code get's KILLED somewhere between two prints in here on 3rd iteration

				myfile.read(reinterpret_cast<char*>(&char_buffer), sizeof(char));
				printf("Ascii code is %d\n", char_buffer);
				while (char_buffer != '\n') {
					caption.push_back(char_buffer);
					myfile.read(reinterpret_cast<char*>(&char_buffer), sizeof(char));
				}

				//reset for further iterations
				ciffH.caption = "";
				for (auto c : caption) {
					ciffH.caption += c;
				}

				std::cout << "Caption: " << ciffH.caption << std::endl;

				int tags_length = ciffH.header_size - sizeof(ciffH.magic) - sizeof(ciffH.header_size) - sizeof(ciffH.content_size);
				tags_length -= sizeof(ciffH.height) + sizeof(ciffH.width) + ciffH.caption.length(); // +1 for \n which is not appended to the caption which is cancelled with the \0 of magic

				std::cout << "Tags length: " << tags_length << std::endl;

				// read tags
				std::string tag;
				ciffH.tags.clear();
				for (int i = 0; i < tags_length; i++) {
					myfile.read(reinterpret_cast<char*>(&char_buffer), sizeof(char));
					if (char_buffer == '\0') {
						ciffH.tags.push_back(tag);
						tag = "";
					}
					else {
						tag += char_buffer;
					}
				}

				std::cout << "Tags: " << std::endl;
				for (auto s : ciffH.tags) {
					std::cout << "-> " << s << std::endl;
				}
			}
#pragma endregion

#pragma region GeneratePreview
			if (id == 0x3 && h.num_anim != 0) {
				std::cout << "Reading pixels..." << std::endl;
				//read pixels
				std::list<pixel> pixels;
				pixel toAppend;
				for (int i = 0; i < (int)ciffH.content_size; i += 3) {
					//extracting RGB values
					myfile.read(reinterpret_cast<char*>(&char_buffer), sizeof(char));
					toAppend.r = char_buffer;
					myfile.read(reinterpret_cast<char*>(&char_buffer), sizeof(char));
					toAppend.g = char_buffer;
					myfile.read(reinterpret_cast<char*>(&char_buffer), sizeof(char));
					toAppend.b = char_buffer;

					pixels.push_back(toAppend);
				}
				if (!generated) {
					std::cout << "Building preview ..." << std::endl;
					//for(auto p : pixels){
					// std::cout << "{" << (int)p.r << ", " << (int)p.g << ", " << (int)p.b << "}" << std::endl;
					//}
					generated = true;
					//building image from pixels
					image << "P3" << std::endl;
					image << ciffH.width << " " << ciffH.height << std::endl;
					image << "255" << std::endl;

					for (int i = 0; i < (int)ciffH.height; i++) {
						for (int j = 0; j < (int)ciffH.width; j++) {
							image << (int)pixels.front().r << " " << (int)pixels.front().g << " " << (int)pixels.front().b << std::endl;
							pixels.pop_front();
						}
					}
					image.close();
				}

			}
#pragma endregion
		}

		myfile.close();
	}
	return 0;
}