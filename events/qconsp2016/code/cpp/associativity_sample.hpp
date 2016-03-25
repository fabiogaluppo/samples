//Sample provided by Fabio Galuppo  
//March 2016 

#ifndef _associativity_sample_
#define _associativity_sample_

#include <iterator>
#include <cassert>
#include <algorithm>

#define SemigroupOperation typename
#define RandomAccessIterator typename

template<RandomAccessIterator I, SemigroupOperation F>
typename I::value_type sequential_scan(I first, I last, F op)
{
	assert(std::distance(first, last) > 0);

	auto result = *first;
	++first;
	while (first != last) 
	{
		result = op(result, *first);
		++first;
	}
	return result;
}

template<RandomAccessIterator I, SemigroupOperation F>
typename I::value_type divide_and_conquer_scan(I first, I last, F op)
{
	static size_t THRESHOLD = 2; //for demonstration purpose

	size_t n = std::distance(first, last);
	assert(n > 0);
	
	if (n >= THRESHOLD)
	{
		n /= 2; //divide into halves
		auto a = divide_and_conquer_scan(first, first + n, op);
		auto b = divide_and_conquer_scan(first + n, last, op);
		return op(a, b); //combine results
	}
	
	return sequential_scan(first, last, op);
}

#include <vector>
#include <functional>

#include <iostream>
#include <iomanip>
using std::cout;
using std::boolalpha;

int associativity_sample_main()
{
	std::vector<int> xs {10, 3, 17, 8, 2, 5, 1, 20, 9};

	//min(1, min(2, 3)) == min(min(1, 2), 3) - associativity
	int min_elem = sequential_scan(xs.cbegin(), xs.cend(), [](int a, int b) { return std::min(a, b); });
	
	//int acc = sequential_scan(xs.cbegin(), xs.cend(), [](int a, int b) { return a + b; }); //lambda version
	int acc = sequential_scan(xs.cbegin(), xs.cend(), std::plus<int>()); //functor version	

	int min_elem2 = divide_and_conquer_scan(xs.cbegin(), xs.cend(), [](int a, int b) { return std::min(a, b); });
	//((a + b) + (c + d)) + ((d + e) + (f + g)) == ((((((a + b) + c) + d) + e) + f) + g) - associativity
	int acc2 = divide_and_conquer_scan(xs.cbegin(), xs.cend(), std::plus<int>());

	cout << min_elem << " == " << min_elem2 << "? " << boolalpha << (min_elem == min_elem2) << "\n";
	cout << acc << " == " << acc2 << "? " << boolalpha  << (acc == acc2) << "\n";

	return 0;
}

#endif /* _associativity_sample_ */

