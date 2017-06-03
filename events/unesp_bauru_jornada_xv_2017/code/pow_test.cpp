//Sample provided by Fabio Galuppo  
//June 2017

//compile: 
//g++ -pipe -Wall -O2 -std=c++14 pow_test.cpp

//emit assembly intel syntax:
//g++ -pipe -Wall -O2 -std=c++14 -S -masm=intel pow_test.cpp

//compile: 
//clang++ -Wall -O2 -std=c++14 pow_test.cpp

//emit assembly intel syntax:
//clang++ -Wall -O2 -std=c++14 -S -masm=intel pow_test.cpp

unsigned long long pow(unsigned int base, unsigned int exponent) {
    long long result = 1;
    while (exponent-- > 0) {
        result *= base;        
    }
    return result;
}

#include <iostream>

int main() {
    std::cout << "2^4 = " << pow(2, 4) << "\n";
    std::cout << "3^3 = " << pow(3, 3) << "\n";
    std::cout << "4^3 = " << pow(4, 3) << "\n";
    std::cout << "5^2 = " << pow(5, 2) << "\n";
    return 0;
}