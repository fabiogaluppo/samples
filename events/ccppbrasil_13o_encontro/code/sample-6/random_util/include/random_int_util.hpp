//Sample provided by Fabio Galuppo 
//October 2016
//http://www.simplycpp.com 

#ifndef RANDOM_INT_UTIL_HPP
#define RANDOM_INT_UTIL_HPP

#include "random_engine_util.hpp"
#include <type_traits>

namespace internals
{
	template<typename Integer>
	using int_distribution = std::uniform_int_distribution<Integer>; //default int distribution

	template<typename Integer>
	struct rand_int_func
	{
		static_assert(std::is_integral<Integer>::value && sizeof(Integer) > 1, 
			"Integer must be an integral type greater than 1 byte");

		int_distribution<Integer> distribution;

		Integer operator()()
		{
			return distribution(rand_engine());
		}

		//compatible with uniform_int_distribution (our default int_distribution)
		Integer operator()(Integer min_inclusive, Integer max_inclusive)
		{
			using param_type = typename int_distribution<Integer>::param_type;
			return distribution(rand_engine(), param_type(min_inclusive, max_inclusive));
		}

		Integer operator()(typename int_distribution<Integer>::param_type params)
		{
			return distribution(rand_engine(), params);
		}
	};

	template<typename Integer>
	inline rand_int_func<Integer>& get_rand_int()
	{
		static thread_local rand_int_func<Integer> rand_int;
		return rand_int;
	}
}

template<typename Integer>
inline Integer rand_int()
{
	return internals::get_rand_int<Integer>()();
}

//compatible with uniform_int_distribution
template<typename Integer>
inline Integer rand_int(Integer min_inclusive, Integer max_inclusive)
{
	return internals::get_rand_int<Integer>()(min_inclusive, max_inclusive);
}

template<typename Integer>
inline Integer rand_int(typename internals::int_distribution<Integer>::param_type params)
{
	return internals::get_rand_int<Integer>()(params);
}

inline int rand_int()
{
	return rand_int<int>();
}

inline int rand_int(int min_inclusive, int max_inclusive)
{
	return rand_int<>(min_inclusive, max_inclusive);
}

#endif /* RANDOM_INT_UTIL_HPP */