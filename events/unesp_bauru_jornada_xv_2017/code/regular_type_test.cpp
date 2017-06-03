//Sample provided by Fabio Galuppo  
//June 2017

//compile:
//clang++ -std=c++14 -O3 regular_type_test.cpp

#include "regular_type.hpp"

#include <iostream>

template <PositiveInteger T>
static inline std::ostream& operator<< (std::ostream& os, const straight_line_segment_1d<T>& val) 
{
	os << val.to_string();
	return os;
}

#include <vector>
#include <algorithm>

int main() 
{
    using straight_line = straight_line_segment_1d<short>; //type alias with template instantiation 

	straight_line ab(2, 4); //constructor
	straight_line cd; //default constructor
	straight_line ef; //default constructor and
	ef = straight_line(1, 6); //copy assignment
	straight_line gh(straight_line(0, 2)); //copy constructor

    ab.set_label("AB"); cd.set_label("CD");
    ef.set_label("EF"); gh.set_label("GH");

    std::cout << "Before sort:\n";
    std::cout << ab << "\n" 
              << cd << "\n"
              << ef << "\n" 
              << gh << "\n";

    std::vector<straight_line> lines { ab, cd, ef, gh };
    std::sort(lines.begin(), lines.end());

    std::cout << "After sort:\n";
    for (auto& line : lines)
        std::cout << line << "\n";

    return 0;
}