//Sample provided by Fabio Galuppo 
//August 2014
//Part of Monadic types prototype for C++

#include <vector>
#include <list>
#include <iostream>

#include "basic_functions.hpp"

void run_basic_functions()
{
	std::vector<int> a { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
	
	std::cout << mkString(a) << "\n";
	std::cout << mkString(a, "; ") << "\n";
	std::cout << mkString(a, "[", "; ", "]") << "\n";

	auto xs = take(4, a);
	std::cout << mkString(xs, "[", "; ", "]") << "\n";

	auto ys = drop(3, a);
	std::cout << mkString(ys, "[", "; ", "]") << "\n";

	std::cout << head(a) << "\n";
	std::cout << head(xs) << "\n";
	std::cout << mkString(tail(xs), "[", "; ", "]") << "\n";
	std::cout << head(drop(2, xs)) << "\n";
	std::cout << mkString(init(a), "[", "; ", "]") << "\n";
	std::cout << last(a) << "\n";

	std::list<int> b;
	try
	{
		std::cout << head(b) << "\n";
	}
	catch (const empty_container_exception& e)
	{
		std::cout << "*** Exception: " << e.what() << "\n";
	}

	b.push_front(1);
	b.push_front(2);
	b.push_front(3);
	b.push_front(4);

	auto s = mkString(b, "[", "; ", "]");
	std::cout << "head of " << s << " = " << head(b) << "\n";
	std::cout << "init of " << s <<" = " << mkString(init(b), "[", "; ", "]") << "\n";
	std::cout << "tail of " << s << " = " << mkString(tail(b), "[", "; ", "]") << "\n";
	std::cout << "last of " << s <<  " = " << last(b) << "\n";

	iter([](const int x) { std::cout << x << " "; }, b);
	std::cout << "\n";

	iteri([](const size_t idx, const int x) { std::cout << idx << " : " << x << "\n"; }, b);

	std::cout << fold([](const int acc, const int x) { return acc + x; }, 0, xs) << "\n";
}