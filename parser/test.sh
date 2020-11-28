#run with source test.sh
make build-test
export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:/home/marcix/CompSecBME/parser/Release
afl-g++ -L/home/marcix/CompSecBME/parser/Release -Wall -O2 main.cpp -lcaff -o test.bin
afl-fuzz -i ./test_files -o output test.bin @@
