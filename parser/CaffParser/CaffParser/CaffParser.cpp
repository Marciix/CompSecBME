#include"CaffParser.h"

int ParseAndValidateCaff(std::string tempDir, std::string validCaffDir, std::string previewDir, std::string fileName) {
	//open file, check whether it exists or not
	#pragma region OpenCaffFile
		std::string pathToCaff;
		pathToCaff.append(tempDir);
		pathToCaff.append(fileName);
		pathToCaff.append(".caff");
		std::ifstream caffFile;
	#pragma endregion

	#pragma region ReadCaff
		caffFile.open(pathToCaff.c_str(), std::ios::in | std::ios::binary);
		uint8_t magic;
		uint64_t length;
		if (caffFile) {
			//read magic byte
			caffFile.read(reinterpret_cast<char*>(&magic), sizeof(magic));
			//read length
			caffFile.read(reinterpret_cast<char*>(&length), sizeof(length));
		}
	#pragma endregion



}