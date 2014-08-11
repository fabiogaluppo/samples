//Sample provided by Fabio Galuppo 
//August 2014
//Part of Monadic types prototype for C++

#include <vector>
#include <iostream>

#include "enumerable_monad.hpp"

enumerable_monad<int, std::vector> get_enumerable_filtered(const std::vector<int>& xs)
{
	auto a = make_enumerable(xs);
	return a.where([](int i) { return i >= 3; })
		    .where([](int i) { return i <= 8; })
			.where([](int i) { return i >= 5; });
}

void run_enumerable_monad()
{
	std::vector<int> xs = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
	auto ys = get_enumerable_filtered(xs);
	
	auto a = make_enumerable(xs);

	auto b = ys.to_container();

	auto c = a.where([](int i) { return i >= 3; })
			  .where([](int i) { return i <= 8; })
			  .select_many<float>([](int i) {
				std::vector<float> v;
				v.push_back(i * 10.f);
				return v; })
			  .to_container();

	auto d = a.where([](int i) { return i >= 3; })
		      .to_container();

	auto e = ys.aggregate(0, [](int acc, int x) { return acc + x; });

	auto f = ys.where([](int i) { return i <= 6; })
			   .aggregate(1.f, [](float acc, int x) { return acc * x; });

	auto g = ys.sum();

	ys.for_each([](int i) { std::cout << i << " "; });
}