//Sample provided by Fabio Galuppo 
//August 2014
//Part of Monadic types prototype for C++

//Roughly these headers are the declarative (functional-like) algorithms offered by STL
#include <functional>
#include <numeric>
#include <algorithm>
//

#include <array>
#include <iostream>
#include <iomanip>

template <class ForwardIterator>
void display(ForwardIterator first, ForwardIterator last)
{
	for (auto it = first; it != last; ++it)
		std::cout << std::setw(2) << *it << " ";
	std::cout << "\n";
}

void run_basic_stl_functional()
{
	std::array<int, 26> xs;
	std::iota(xs.begin(), xs.end(), 1);

	display(xs.begin(), xs.end());

	auto is_even = [](int x) { return (x & 0x1) == 0x0; };
	
	auto bound_iterator = std::partition(xs.begin(), xs.end(), is_even);
	
	display(xs.begin(), bound_iterator);
	display(bound_iterator, xs.end());
	display(xs.begin(), xs.end()); //after partition

	std::random_shuffle(xs.begin(), xs.end());
	
	std::array<int, 13> evens, odds;
	std::partition_copy(xs.begin(), xs.end(), evens.begin(), odds.begin(), is_even);

	display(evens.begin(), evens.end());
	display(odds.begin(), odds.end());
	display(xs.begin(), xs.end()); //after random_shuffle

	std::array<int, 13> ys;
	auto sum_func = [](int lhs, int rhs) { return lhs + rhs; };
	std::transform(evens.begin(), evens.end(), odds.begin(), ys.begin(), std::bind(sum_func, std::placeholders::_1, std::placeholders::_2));
	
	display(ys.begin(), ys.end()); //after transform

	std::sort(ys.begin(), ys.end());
	
	display(ys.begin(), ys.end()); //after sort

	auto total = std::accumulate(ys.begin(), ys.end(), 0, std::plus<int>());
	std::cout << total << "\n";

	auto it = xs.begin();
	while ((it = std::find_if(it, xs.end(), is_even)) != xs.end())
	{
		std::cout << std::setw(2) << *it << " is even!" << "\n";
		++it;
	}
}