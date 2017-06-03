//Sample provided by Fabio Galuppo
//November 2015
//http://www.simplycpp.com

//compile with clang++:
//clang++ -Wall -O2 -std=c++11 -o understanding_iota_program.exe understanding_iota_program.cpp

//compile g++:
//g++ -Wall -O2 -std=c++11 -o understanding_iota_program.exe understanding_iota_program.cpp

//compile msvc:
//cl.exe / Wall / EHsc / Ox understanding_iota_program.cpp / link / out:understanding_iota_program.exe

#include "understanding_iota.hpp"
#include <algorithm>
#include <chrono>

void understanding_iota_run()
{
	std::vector<int> xs;
	xs.resize(10);

	for (size_t i = 0 /* init */; i < xs.size() /* range end */; ++i /* increment */)
		xs[i] = static_cast<int>(i) + 1;
	display(xs);

	std::iota(xs.begin(), xs.end(), 1);
	display(xs);

	std::iota(xs.begin(), xs.end(), 10);
	display(xs);

	std::fill(xs.begin(), xs.end(), 0);
	std::iota(xs.begin(), xs.begin() + xs.size() / 2, 1);
	display(xs);

	std::iota(xs.rbegin(), xs.rend(), 0);
	display(xs);

	std::iota(xs.rbegin(), xs.rend(), 100);
	display(xs);

	std::iota(xs.begin(), xs.end(), iota_step<int>(2, 2));
	display(xs);

	std::iota(xs.begin(), xs.end(), iota_step<int>(0, -2));
	display(xs);

	std::iota(xs.begin(), xs.end(), make_iota_step(100, 100));
	display(xs);

	std::iota(xs.begin(), xs.end(), make_iota_random_with_uniform_distribution(1, 10));
	display(xs);

	unsigned int seed = static_cast<unsigned int>(std::chrono::system_clock::now().time_since_epoch().count());
	std::iota(xs.begin(), xs.end(), make_iota_random_with_uniform_distribution(0, 255, seed));
	display(xs);
}

int main()
{
	understanding_iota_run();
	return 0;
}

/*

Output example:

1 2 3 4 5 6 7 8 9 10
1 2 3 4 5 6 7 8 9 10
10 11 12 13 14 15 16 17 18 19
1 2 3 4 5 0 0 0 0 0
9 8 7 6 5 4 3 2 1 0
109 108 107 106 105 104 103 102 101 100
2 4 6 8 10 12 14 16 18 20
0 -2 -4 -6 -8 -10 -12 -14 -16 -18
100 200 300 400 500 600 700 800 900 1000
3 3 5 6 5 2 10 6 9 4
74 90 135 254 152 178 57 35 103 85

*/
