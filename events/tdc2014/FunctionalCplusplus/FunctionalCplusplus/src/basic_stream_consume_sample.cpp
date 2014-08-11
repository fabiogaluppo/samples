//Sample provided by Fabio Galuppo 
//August 2014
//Part of Monadic types prototype for C++

#include "stream.hpp"

#include <numeric>
#include <vector>
#include <iostream>

void run_basic_stream_consume()
{
	std::vector<int> xs(10);
	std::iota(xs.begin(), xs.end(), 1);

	auto m = make_stream(xs);
	m.map([](int x) { return 10 * x; }) //in terms of std::transform
	 .filter([](int x) { return x >= 50; }) //in terms of std::copy_if
	 .for_each([](int x) { std::cout << x << " "; }); //in terms of ranged-for
}