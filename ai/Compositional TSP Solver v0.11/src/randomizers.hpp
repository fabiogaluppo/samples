//Sample provided by Fabio Galuppo
//Compositional TSP Solver version 0.11
//September 2014

#pragma once
#ifndef _randomizers_
#define _randomizers_

#include <memory>
#include <random>
#include <ctime>

struct random_functor final
{
	random_functor(int min_inclusive, int max_inclusive, unsigned seed = static_cast<unsigned>(std::time(nullptr))) : 
		Engine_(new std::default_random_engine(seed)),
		Rnd_(new std::uniform_int_distribution<int>(min_inclusive, max_inclusive))
	{
	}

	random_functor(int max_exclusive, unsigned seed = static_cast<unsigned>(std::time(nullptr))) :
		Engine_(new std::default_random_engine(seed)),
		Rnd_(new std::uniform_int_distribution<int>(0, max_exclusive - 1))
	{
	}

	//[0; max_inclusive]
	auto operator()() const -> int { return (*Rnd_)(*Engine_); }

	random_functor() = delete;
	random_functor(const random_functor&) = default;
	random_functor& operator=(const random_functor&) = delete;

private:
	std::shared_ptr<std::default_random_engine> Engine_;
	std::shared_ptr<std::uniform_int_distribution<int>> Rnd_;
};

struct double_random_functor final
{
	double_random_functor(unsigned seed = static_cast<unsigned>(std::time(nullptr))) : 
		Engine_(new std::default_random_engine(seed)), 
	    Rnd_(new std::uniform_real_distribution<double>())
	{
	}

	//[0.0; 1.0]
	auto operator()() const -> double  { return (*Rnd_)(*Engine_); }

	double_random_functor(const double_random_functor&) = default;
	double_random_functor& operator=(const double_random_functor&) = delete;

private:
	std::shared_ptr<std::default_random_engine> Engine_;
	std::shared_ptr<std::uniform_real_distribution<double>> Rnd_;
};

#endif /* _randomizers_ */