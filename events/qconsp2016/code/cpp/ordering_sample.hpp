//Sample provided by Fabio Galuppo  
//March 2016 

#ifndef _ordering_sample_
#define _ordering_sample_

template<typename T>
//T - Strict Totally Ordering
inline const T& my_min(const T& a, const T& b)
{
	if (b < a) return b;
	return a;
}

template<typename T, typename R>
//T - Any Type
//R - Strict Weak Ordering on T
inline const T& my_min(const T& a, const T& b, R relation)
{
	if (relation(b, a)) return b;
	return a;
}

template<typename T0, typename T1>
struct my_ordered_pair
{
	T0 first; T1 second;

	bool operator==(const my_ordered_pair<T0, T1>& that)
	{
		if (this == &that) 
			return true;
		return first == that.first && second == that.second;
	}
};

template<typename T0, typename T1>
struct less_my_ordered_pair_1st 
{
	bool operator()(const my_ordered_pair<T0, T1>& a, 
		const my_ordered_pair<T0, T1>& b)
	{
		return a.first < b.first;
	}
};

template<typename T0, typename T1>
struct less_my_ordered_pair_2nd
{
	bool operator()(const my_ordered_pair<T0, T1>& a, 
		const my_ordered_pair<T0, T1>& b)
	{
		return a.second < b.second;
	}
};

#include "my_swap_sample.hpp"
#include <cassert>

template<typename T>
//T - Strict Totally Ordering
inline void stable_sort_2(T& a, T& b)
{
	if (b < a) my_swap(a, b);

	//post-condition:
	assert(a == my_min(a, b)); //equality and min
}

template<typename T>
inline T& mutable_median_of_3(T& a, T& b, T& c)
{
	stable_sort_2(a, b);
	//assert(a <= b);
	
	if (c < b)
	{
		stable_sort_2(a, c);
		//assert(a <= c);

		return c;
	}
	return b;
}

template<typename T>
inline const T& immutable_median_of_3(const T& a, const T& b, const T& c)
{
	//best case : 2 comparisons
	//worst case: 3 comparisons
	//average case (using expected value from probability theory):
	//1/3 a + 1/3 b + 1/3 c
	//a greatest = 3 comparisons
	//b greatest = 3 comparisons
	//c greatest = 2 comparisons
	//1/3 (3) + 1/3 (3) + 1/3 (2)
	//1 + 1 + 2/3
	//(2 + 2/3) comparisons
	
	if (b < a)
	{
		if (c < a)
			return std::max(b, c); //3 comparisons //a > (b || c)
		//a < c
		return a; //2 comparisons //c > (a || b)
	}	
	
	//a < b
	if (c < b)
		return std::max(a, c); //3 comparisons //b > (a || c)
	//b < c
	return b; //2 comparisons //c > (a || b)
}

#include <iostream>
#include <iomanip>
#include <utility>

int ordering_sample_main()
{
	using std::cout;
	using std::cin;
	using std::boolalpha;

	/*
	double a, b;
	cin >> a; cin >> b;
	double min_a_b = my_min(a, b);
	cout << "min between " << a << " and " << b << " is " << min_a_b << "\n";
	*/
	{
		my_ordered_pair<int, int> p1{ 6, 7 }, p2{ 5, 8 };
		my_ordered_pair<int, int> p3 = my_min(p1, p2, less_my_ordered_pair_1st<int, int>());
		cout << p3.first << ", " << p3.second << "\n";
	}
	{
		my_ordered_pair<int, int> p1{ 6, 7 }, p2{ 5, 8 };
		my_ordered_pair<int, int> p3 = my_min(p1, p2, less_my_ordered_pair_2nd<int, int>());
		cout << p3.first << ", " << p3.second << "\n";

		//equality
		cout << boolalpha << (p1 == p1) << "\n";
		cout << boolalpha << (p1 == p2) << "\n";
		cout << boolalpha << (p1 == my_ordered_pair<int, int>{ 6, 7 }) << "\n";
	}
	{
		std::pair<int, int> p1{ 6, 7 }, p2{ 5, 8 };
		std::pair<int, int> p3 = my_min(p1, p2);
		cout << p3.first << ", " << p3.second << "\n";
	}
	{
		std::pair<int, int> p1{ 100, 1 }, p2{ 1, 100 };
		stable_sort_2(p1, p2);
		cout << p1.first << ", " << p1.second << "\n";
	}
	{
		int a[]{ 1, 2, 3, 4, 5 };
		int m = mutable_median_of_3(a[3], a[0], a[1]); //{3, 1, 2}
		cout << m << "\n"; //2
		m = mutable_median_of_3(a[2], a[3], a[4]); //{3, 1, 5}
		cout << m << "\n"; //3
		
		std::iota(a, a + (sizeof(a) / sizeof(a[0])), 1); //{1, 2, 3, 4, 5}
		m = immutable_median_of_3(a[3], a[0], a[1]); //{3, 1, 2}
		cout << m << "\n"; //2
		m = immutable_median_of_3(a[2], a[3], a[4]); //{3, 4, 5}
		cout << m << "\n"; //4
	}
	return 0;
}

#endif /* _ordering_sample_ */