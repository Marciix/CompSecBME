#include"CaffParser.h"
#include<iostream>

int main(int argc, char* argv[]){
	if(argc < 2){
		std::cout << "Entered less than 1 arguments, exiting." << std::endl;
		exit(0);
	}
	if(argc > 2){
		std::cout << "Entered more than 1 arguments, exiting." << std::endl;
		exit(0);
	}
    
    int ret = ParseAndValidateCaff(argv[1], "./preview.ppm", "./info.json");
    return ret;
}