//Sample provided by Fabio Galuppo
//November 2015
//http://www.simplycpp.com

#include <numeric>
#include <vector>
#include <iostream>

//std::iota Ref.: http://www.cplusplus.com/reference/numeric/iota/

template <class Iter>
void display(Iter& iter)
{
	for (auto i : iter) std::cout << i << " ";
	std::cout << "\n";
}

template<typename T>
struct iota_step final
{
	explicit iota_step(T init, const T increment) 
		: next(init), step(increment) {}

	iota_step& operator++()
	{
		next += step;
		return *this;
	}

	operator T() const { return next; }

	iota_step() = delete; //default-constructible	
	iota_step(const iota_step&) = default; //copy-constructible
	iota_step& operator=(const iota_step&) = default; //copy-assignable
	~iota_step() = default; //destructor

private:
	T next;
	const T step;
};

template<typename T>
iota_step<T> make_iota_step(T init, const T increment) { return iota_step<T>(init, increment); }

#include <random>
#include <functional>

template<typename T>
struct iota_random_with_uniform_distribution final
{
	explicit iota_random_with_uniform_distribution(const T minInclusive, const T maxInclusive, unsigned int seed = 5489U)
		: rnd(std::bind(std::uniform_int_distribution<T>(minInclusive, maxInclusive), std::default_random_engine(seed)))
	{
		next = rnd();
	}

	iota_random_with_uniform_distribution& operator++()
	{
		next = rnd();
		return *this;
	}

	operator T() const { return next; }

	iota_random_with_uniform_distribution() = delete; //default-constructible	
	iota_random_with_uniform_distribution(const iota_random_with_uniform_distribution&) = default; //copy-constructible
	iota_random_with_uniform_distribution& operator=(const iota_random_with_uniform_distribution&) = default; //copy-assignable
	~iota_random_with_uniform_distribution() = default; //destructor

private:
	T next;
	std::function<T(void)> rnd;
};

template<typename T>
iota_random_with_uniform_distribution<T> make_iota_random_with_uniform_distribution(const T minInclusive, const T maxInclusive, unsigned int seed = 5489U)
{ 
	return iota_random_with_uniform_distribution<T>(minInclusive, maxInclusive, seed);
}
