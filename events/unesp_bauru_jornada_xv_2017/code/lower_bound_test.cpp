//Sample provided by Fabio Galuppo  
//June 2017

//compile: 
//g++ -pipe -Wall -O2 -std=c++1z lower_bound_test.cpp

//compile: 
//clang++ -Wall -O2 -std=c++1z lower_bound_test.cpp

#include "lower_bound.hpp"

#include <vector>
#include <forward_list>
#include <numeric>
#include <iostream>

template <class Iter>
void display(Iter& iter)
{
    std::cout << "\n";
    for (auto i : iter) 
        std::cout << i << " ";
    std::cout << "\n";
}

int main() 
{
    std::vector<int> xs(10, 0);
    std::iota(xs.begin(), xs.end(), 10);
    
    display(xs);
    auto low = lower_bound(xs.begin(), xs.end(), 15);
    std::cout << "lower_bound of 15 at position " << (low - xs.begin()) << '\n';

    *(low - 1) = 15;
    display(xs);
    low = lower_bound(xs.begin(), xs.end(), 15);
    std::cout << "lower_bound of 15 at position " << (low - xs.begin()) << '\n';

    low = lower_bound(xs.begin(), xs.end(), 20);
    if (low != xs.end())
        std::cout << "lower_bound of 20 at position " << (low - xs.begin()) << '\n';
    else
        std::cout << "lower_bound of 20 not found\n";

    std::forward_list<int> ys(10, 0);
    std::iota(ys.begin(), ys.end(), 50);

    display(ys);
    auto low2 = lower_bound(ys.begin(), ys.end(), 51);
    std::cout << "lower_bound of 51 " <<  (low2 == ys.end() ? "not " : "") << "found\n";

    low2 = lower_bound(ys.begin(), ys.end(), 73);
    std::cout << "lower_bound of 73 " <<  (low2 == ys.end() ? "not " : "") << "found\n";
}