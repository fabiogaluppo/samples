//Sample provided by Fabio Galuppo  
//March 2016 

//msvc (Visual C++) compile: 
//cl /EHsc /Ox program.cpp

//g++ (gcc) compile:
//g++ -std=c++11 -O3 -o program.exe program.cpp

#include "my_swap_sample.hpp"
#include "accumulate_sample.hpp"
#include "ordering_sample.hpp"
#include "group_sample.hpp"
#include "associativity_sample.hpp"
#include "regular_type_sample.hpp"
#ifndef __MINGW32__
#include "policy_sample.hpp"
#endif

#include <iostream>
#include <string>

void print_separator(std::string title = "")
{
	std::cout << title << " " << std::string(41 - title.size(), '-') << "\n";
}

int main()
{
	my_swap_sample_main(); print_separator("my_swap");
	accumulate_sample_main(); print_separator("accumulate");
	ordering_sample_main(); print_separator("ordering");
	group_sample_main(); print_separator("group");
	associativity_sample_main(); print_separator("associativity");
	regular_type_sample_main(); print_separator("regular_type");
	#ifndef __MINGW32__
	policy_sample_main(); print_separator("policy");
	#endif

	return 0;
}