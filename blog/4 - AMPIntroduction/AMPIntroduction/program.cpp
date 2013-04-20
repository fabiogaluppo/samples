//Sample provided by Fabio Galuppo
#include <windows.h>
#include <iostream>
#include <iterator>
#include <algorithm>

#include "CPPRefresh.hpp"
#include "ArrayView.hpp"
#include "ParallelForEach.hpp"
#include "Tiling.hpp"
#include "Computation.hpp"
#include "BlackScholes.hpp"
#include "Accelerator.hpp"

int main()
{
	using std::cout;
	using std::cin;
	using std::ostream_iterator;
	using std::copy;

	const char* samples[] =
	{
		"1 - C++ Refresh",
		"2 - array_view",
		"3 - parallel_for_each",
		"4 - tile_static",
		"5 - Computation C++ AMP Math",
		"6 - Black-Scholes Model",
		"7 - Accelerator"
	};
	
	SetConsoleTitle(L"C++ AMP Samples - Fabio Galuppo");

	while(true)
	{
		copy(samples, samples + _countof(samples), ostream_iterator<const char*>(cout, "\n")); 
		cout << "\nchoose sample [1 - " << _countof(samples) << "]: ";
		
		int n;
		cin >> n;

		cout << "\n";

		switch(n)
		{
		case 1:
			CPPRefresh::run();
			break;

		case 2:
			ArrayView::run();
			break;

		case 3:
			ParallelForEach::run();
			break;

		case 4:
			Tiling::run();
			break;

		case 5:
			Computation::run();
			break;

		case 6:
			BlackScholes::run();
			break;

		case 7:
			Accelerator::run();
			break;
	
		default:
			return 0;
		}

		cout << "\n";
	}
}