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
	mkdir -p  Release 
	$(CC) $(CFLAGS) $(SOURCE) -o Release/CaffParser.so

clean-linux:
	rm -f Release/CaffParser.so

build-test:
	mkdir -p  Release 
	$(CC) $(CFLAGS) $(SOURCE) -o Release/libcaff.so

clean-test:
	rm -f Release/libcaff.so