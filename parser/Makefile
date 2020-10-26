CC = g++
CFLAGS = -shared -fPIC
WINFLAG = -DBUILD_DLL
SOURCE = CaffParser.h CaffParser.cpp

build-windows:
	mkdir Release 
	$(CC) $(WINFLAG) $(CFLAGS) $(SOURCE) -o Release\CaffParser.dll
	 
clean-windows:
	del Release\CaffParser.dll
	
build-linux:
	mkdir Release 
	$(CC) $(CFLAGS) $(SOURCE) -o Release/CaffParser.so

clean-linux:
	rm Release/CaffParser.so