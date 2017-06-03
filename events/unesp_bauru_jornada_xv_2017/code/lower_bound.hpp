//Sample provided by Fabio Galuppo  
//June 2017

#ifndef LOWER_BOUND_HPP
#define LOWER_BOUND_HPP

#include <iterator>
#include <type_traits>

#define ForwardIterator typename
#define Compare typename

template<typename T>
using DifferenceType = typename std::iterator_traits<T>::difference_type;

template<typename N>
inline N half(N n) 
{ 
	static_assert(std::is_integral<N>(), "not integral type");
	return n >> 1; 
}

template<ForwardIterator I, typename T, Compare C>
I lower_bound(I first, I last, const T& val, C compare)
{
	using std::distance;
	using std::advance;
	using D = DifferenceType<I>;
	
	D count = distance<>(first, last);
	while (count > D(0))
	{
		I it = first;
		D step = half<>(count);
		advance<>(it, step);
		if (compare(it, val))
		{
			first = it;
			++first;
			count = count - step - D(1);
		}
		else
		{
			count = step;
		}
	}
	return first;
}

#endif /* LOWER_BOUND_HPP */