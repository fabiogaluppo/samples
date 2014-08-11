//Sample provided by Fabio Galuppo 
//August 2014
//Part of Monadic types prototype for C++

#include <vector>
#include <list>
#include <string>
#include <iostream>

#include "container_monad.hpp"

void run_container_monad()
{
	std::vector<int> a = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

	auto b = unit(a);

	auto c = b.bind([](int i){
		std::vector<int> result = { i * 2 };
		return result;
	});

	auto d = b.bind<float>([](int i){
		std::vector<float> result = { i * 1.5f };
		return result;
	});

	auto e = b.bind<float, std::list>([](int i){
		std::list<float> result = { i * 2.5f };
		return result;
	});

	std::vector<std::string> xs = { "hello", "world" };
	auto v = unit(xs);
	auto chars = v.bind<char>([](const std::string& s){
		std::vector<char> temp;
		temp.resize(s.size());
		std::copy(s.cbegin(), s.cend(), temp.begin());
		return temp;
	});
}