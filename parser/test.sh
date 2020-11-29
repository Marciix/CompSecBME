#run with source test.sh
make build-test
export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:/home/martonbak/CompSecBME/parser/Release
/home/martonbak/Tools/afl-2.52b/afl-g++ -L/home/martonbak/CompSecBME/parser/Release -Wall -O2 main.cpp -lcaff -o parser.bin
