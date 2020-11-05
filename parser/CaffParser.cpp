#include"CaffParser.h"
#include<string>
#include<iostream>
#include<fstream>
#include<stdint.h>
#include<list>
#include<stdio.h>
#include<string.h>

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

int ParseAndValidateCaff(const char* _caffDir, const char* _previewDir, const char* _jsonDir) {
	
	std::string caffDir = _caffDir;
	std::string previewDir = _previewDir;
	std::string jsonDir = _jsonDir;

	//variables
	uint8_t id;
	//bools for: if we detected given ID yet or not
	bool r1 = false;
	bool r2 = false;
	bool r3 = false;
	uint64_t length;
	char char_buffer;
	bool generated = false;
	bool infogen = false;
	uint64_t duration;
	ciffHeader ciffH;
	caffHeader h;
	std::list<char> caption;
	int anims_read = 0;

	std::ifstream myfile;
	std::ofstream image(previewDir.c_str());
	std::ofstream json(jsonDir.c_str());

	myfile.open(caffDir.c_str(), std::ios::in | std::ios::binary);

	if (myfile) {
		while (myfile.good() && !myfile.eof()){
#pragma region WhichBlock
			myfile.read(reinterpret_cast<char*>(&id), sizeof(uint8_t)); //read id byte
			if (id > 3) {
				return 1;
			}
			std::cout << "ID: " << unsigned(id) << std::endl;

			myfile.read(reinterpret_cast<char*>(&length), sizeof(uint64_t));
			std::cout << "Length: " << length << std::endl;
			if(length < 14){  //14-bytes is the shortest possible valid block
				return 1;
			}

			//if read all necessary elements, and no CIFF section is left, return 0
			if(r1 && r2 && r3 && h.num_anim == 0){
				return 0;
			}
#pragma endregion

#pragma region ID1
			if (id == 0x1) {
				//confirm we recieved ID1 section
				r1 = true;
				//myfile.read(reinterpret_cast<char*>(&buffer), length);
				myfile.read(reinterpret_cast<char*>(&h.magic), 4);

				// Appending \0 to CAFF magic header
				h.magic[4] = '\0';

				int comp = strcmp(h.magic, "CAFF\0");
				if(comp != 0){
					return 1;
				}
				std::cout << "Comparing magic (0 if equal): " << comp << std::endl;


				myfile.read(reinterpret_cast<char*>(&h.header_size), 8);
				
				//checking if header_size and length param match up
				if(h.header_size != length){
					return 1;
				}

				myfile.read(reinterpret_cast<char*>(&h.num_anim), 8);

				//restore balance to the force
				h.num_anim -= anims_read;

				std::cout << "Magic: " << h.magic << std::endl;
				std::cout << "Header size: " << h.header_size << std::endl;
				std::cout << "Num of anim: " << h.num_anim << std::endl;
				
				// Always 20: magic : 4 + 2*8 for header_size and num_anim
				if(length != 20){
					return 1;
				}
				std::cout << "Size of H: " << sizeof(h.magic) - 1 + 2* sizeof(h.header_size) << std::endl;
			}
#pragma endregion

#pragma region ID2
			if (id == 0x2) {
				//confirm we recieved ID2 section
				r2 = true;

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

				//checking if size is equal to length
				if(length != (14 + creator_len)){ // 14 is the date (6 bytes) + 8 byte integer
					return 1;
				}
			}
#pragma endregion

#pragma region ID3
			if (id == 0x3 && h.num_anim != 0) {
				//confirm we recieved ID3
				r3 = true;

				anims_read++;
				h.num_anim--;
				myfile.read(reinterpret_cast<char*>(&duration), sizeof(uint64_t));
				std::cout << "Duration: " << duration << std::endl;

				////////////////////////////////////////////////
				//                 CIFF READ                  //
				////////////////////////////////////////////////
				myfile.read(reinterpret_cast<char*>(&ciffH.magic), 4);

				ciffH.magic[4] = '\0';
				std::cout << "CIFF Magic: " << ciffH.magic << std::endl;
				int comp2 = strcmp(ciffH.magic, "CIFF\0");
				if(comp2 != 0){
					return 1;
				}
				std::cout << "CIFF Header magic compare (0 if equals): " << comp2 << std::endl;

				myfile.read(reinterpret_cast<char*>(&ciffH.header_size), sizeof(uint64_t));
				std::cout << "CIFF Header size: " << ciffH.header_size << std::endl;

				myfile.read(reinterpret_cast<char*>(&ciffH.content_size), sizeof(uint64_t));
				std::cout << "CIFF Content size: " << ciffH.content_size << std::endl;
				
				// matching length to the contents
				if(length != sizeof(duration) + ciffH.header_size + ciffH.content_size){
					return 1;
				}

				myfile.read(reinterpret_cast<char*>(&ciffH.width), sizeof(uint64_t));
				std::cout << "CIFF Width: " << ciffH.width << std::endl;

				myfile.read(reinterpret_cast<char*>(&ciffH.height), sizeof(uint64_t));
				std::cout << "CIFF Height: " << ciffH.height << std::endl;

				if(ciffH.content_size != ciffH.width * ciffH.height *3){
					return 1;
				}
				
				//empty it in case of other type 2 block comes
				caption.clear();

				myfile.read(reinterpret_cast<char*>(&char_buffer), sizeof(char));
				printf("Ascii code of first char is %d\n", char_buffer);
				while (char_buffer != '\n') {
					if(char_buffer == '\n') break;
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
			
				////////////////////////////////////////////////
				//                 WRITE JSON                 //
				////////////////////////////////////////////////
				if(!infogen){
					infogen = true;
					std::string sep = ",";

					json << "{" << std::endl;
					json << "\t\"title\": \"" << ciffH.caption << "\","<< std::endl;
					json << "\t\"tags\": [" << std::endl;
					int count = 0;
					for(auto t: ciffH.tags){
						if(count == ciffH.tags.size()-1){
							json << "\t\t\"" << t << "\"" << std::endl;
						}else{
							json  << "\t\t\"" <<  t  << "\"" << sep << std::endl;
						}
						count++;
					}
					json << "\t]" << std::endl;
					json << "}" << std::endl;
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
					generated = true;
					//building image header
					image << "P3" << std::endl;
					image << ciffH.width << " " << ciffH.height << std::endl;
					image << "255" << std::endl;


					// writing pixel data
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
	// file is wrong
	} else {
		return 1;
	}
	return 0;
}
