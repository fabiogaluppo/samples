//Sample provided by Fabio Galuppo  
//March 2016 

#ifndef _my_swap_sample_
#define _my_swap_sample_

template<typename T>
void my_swap(T& lhs, T& rhs)
{
	T temp = lhs;
	lhs = rhs;
	rhs = temp;
}

#include <iterator>

#define RandomAccessIterator typename

template<RandomAccessIterator T>
/*
RandomAccessIterator required to [], copy, assign
*/
void my_swap_adjacent_pairs(T first, size_t N)
{
	for (size_t i = 1; i < N; i += 2)
		my_swap(first[i - 1], first[i]);
}

#include <iostream>

void swap_from_input()
{
	int a, b;
	std::cout << "input a [int]:"; std::cin >> a;
	std::cout << "input b [int]:"; std::cin >> b;
	std::cout << "before:" << a << " " << b << "\n";
	my_swap(a, b);
	std::cout << "after :" << a << " " << b << "\n";
}

#include <random>
#include <functional>
#include <limits>
#include <ctime>
#include <array>

#define IntType typename
/*
template<class _Ty>
struct _Is_IntType
: _Cat_base<is_same<_Ty, short>::value
|| is_same<_Ty, int>::value
|| is_same<_Ty, long>::value
|| is_same<_Ty, long long>::value
|| _Is_UIntType<_Ty>::value>
{	// determine whether _Ty satisfies <random>'s IntType requirements
};
*/
template <IntType T>
struct randomizer final
{
	//doing the job of a default constructor
	randomizer(unsigned seed = static_cast<unsigned>(std::time(nullptr))) :
		rnd(std::bind(std::uniform_int_distribution<T>(min_value(), max_value()),
			std::default_random_engine(seed))) {}

	randomizer(const randomizer&) = delete; //copy constructor
	randomizer& operator=(const randomizer&) = delete; //copy assignment	
	~randomizer() = default; //default destructor

	T next() { return rnd(); }

private:
	static T min_value() { return std::numeric_limits<T>::min(); }
	static T max_value() { return std::numeric_limits<T>::max(); }

private:
	std::function<T(void)> rnd;
};

void swap_from_array()
{
	randomizer<unsigned short> rnd;

	//fill with random numbers
	std::array<unsigned short, 10> xs;
	for (size_t i = 0; i < xs.size(); ++i)
		xs[i] = rnd.next();

	//print before
	std::cout << "before:";
	for (size_t i = 0; i < xs.size(); ++i) std::cout << xs[i] << " ";
	std::cout << "\n";

	//swap adjacent pairs
	for (size_t i = 1; i < xs.size(); i += 2)
		my_swap(xs[i - 1], xs[i]);

	//print after
	std::cout << "after :";
	for (size_t i = 0; i < xs.size(); ++i) std::cout << xs[i] << " ";
	std::cout << "\n";
}

#include <numeric>

template<IntType T>
/*
T - Domain of IntTypes
*/
struct iota_random final
{
	explicit iota_random(randomizer<T>& rnd) : rnd(rnd)
	{
		next = rnd.next();
	}

	iota_random& operator++()
	{
		next = rnd.next();
		return *this;
	}

	operator T() const { return next; }

private:
	T next;
	randomizer<T>& rnd;
};

#include <algorithm>

template <IntType T, size_t N>
/*
T - Domain of IntTypes
N - size of array
*/
void swap_from_array_no_raw_loops()
{
	randomizer<T> rnd;
	std::array<T, N> xs;

	//http://simplycpp.com/2015/11/06/mestre-iota/
	//fill with random numbers
	std::iota(xs.begin(), xs.end(), iota_random<T>(rnd));

	//print before
	std::cout << "before:";
	std::for_each(xs.begin(), xs.end(), [](const T& x) { std::cout << x << " "; });
	std::cout << "\n";

	//swap adjacent pairs
	my_swap_adjacent_pairs(xs.begin(), N);

	//print after
	std::cout << "after: ";
	std::for_each(xs.begin(), xs.end(), [](const T& x) { std::cout << x << " "; });
	std::cout << "\n";
}

int my_swap_sample_main()
{
	int a = 1, b = 2;
	std::cout << "before:" << a << " " << b << "\n";
	my_swap(a, b);
	std::cout << "after :" << a << " " << b << "\n";

	swap_from_input();

	swap_from_array();

	swap_from_array_no_raw_loops<unsigned short, 10>();

	return 0;
}

#endif /* _my_swap_sample_ */