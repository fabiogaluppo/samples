//Sample provided by Fabio Galuppo 
//August 2014
//Part of Monadic types prototype for C++

#include <vector>
#include <list>
#include <iostream>

#include "stream.hpp"

void run_stream()
{
	std::vector<int> a = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

	auto m = make_stream(a);
	float total = m.reduce(0.f, [](float acc, const int x) { 
		return acc + x; 
	});

	std::cout << total << "\n";

	auto filtered = m.filter([](int i) { return i >= 5; });
	filtered.for_each([](int i) { std::cout << i << " "; });
	std::cout << "\n";

	std::vector<char> b = { 'a', 'b', 'c', 'd', 'e', 'f' };
	auto result1 = m.zip(b);
	auto result2 = make_stream(b).zip(a);

	auto result3 = m.to_container();
	auto result4 = m.to_container<std::list>();

	auto result5 = make_stream(b)
					.sorted([](int i, int j) { return j < i; })
					.to_container();
}