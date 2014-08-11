//Sample provided by Fabio Galuppo 
//August 2014
//Part of Monadic types prototype for C++

#include <iostream>

#include "try_monad.hpp"

void test(const try_monad_ptr<int>& x)
{
	auto result =
	//match<int, void>(x,
	match<int, int>(x,
	[](int i) { //success
		return 1;
		//std::cout << i << "\n";
	},
	[](const std::exception& e) { //failure
		return 0;
		//std::cout << e.what() << "\n";
	});
	std::cout << result << "\n";
    
    x->match<void>(
	[](int i) { //success
		std::cout << i << "\n";
	}, [](const std::exception& e) { //failure
		std::cout << e.what() << "\n";
	});

	x->map<float>([](int i){ return 10.f * i; })
     ->match<void>([](float i) { //success
		std::cout << i << "\n";
	}, [](const std::exception& e) { //failure
		std::cout << e.what() << "\n";
	});
}

void run_try_monad()
{
	test(make_try(1));
	test(make_try<int>(std::runtime_error("error...")));
}