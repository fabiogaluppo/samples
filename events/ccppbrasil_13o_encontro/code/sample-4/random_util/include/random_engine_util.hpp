//Sample provided by Fabio Galuppo 
//October 2016
//http://www.simplycpp.com 

#ifndef RANDOM_ENGINE_UTIL_HPP
#define RANDOM_ENGINE_UTIL_HPP

#include <random>

namespace internals
{
	using random_engine = std::default_random_engine; //default engine
	
	static /* thread_local */ random_engine current_engine;

	inline random_engine::result_type rd()
	{
		return std::random_device{}();
	}

	inline random_engine& rand_engine()
	{
		static /* thread_local */ random_engine engine{ rd() };
		return engine;
	}

	inline void seed_rand()
	{
		rand_engine().seed(rd());
	}

	inline void seed_rand(random_engine::result_type seed)
	{
		rand_engine().seed(seed);
	}
}

inline void seed_rand() 
{
	internals::seed_rand();
}

inline void seed_rand(internals::random_engine::result_type seed)
{
	internals::seed_rand(seed);
}

#endif /* RANDOM_ENGINE_UTIL_HPP */