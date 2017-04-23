//Sample provided by Fabio Galuppo  
//April 2017

//compile:
//clang++ -std=c++14 -I../include -I./cppzmq/include -lzmq program.cpp -o ./bin/inject.exe

//run:
//./bin/inject.exe "tcp://localhost:60000" "tcp://localhost:60001" 10 "DL1"

#include "inject.hpp"
//#include "inject2.hpp"

int main(int argc, const char* argv[]) {
	inject::run(argc, argv);
	//inject2::run(argc, argv);
	return 0;
}
