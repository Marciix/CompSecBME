#pragma once

#ifdef BUILD_DLL
#define CAFFPARSER_API __declspec(dllexport)
#else
#define CAFFPARSER_API __declspec(dllimport)
#endif

/**
* @param tempDir: Full path to directory holding temporary caff files until they are handled
* @param validCaffDir: Full path to directory which holds the valid CAFF files
* @param previewDir: Full path to directory which holds the preview images to CAFF files
* @param fileName: Holds the filename to analyze without the .caff extension 
*/
extern "C" 
{
	CAFFPARSER_API int ParseAndValidateCaff(const char* tempDir, const char* validCaffDir, const char* previewDir, const char* fileName);
}