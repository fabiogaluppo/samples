//Sample provided by Fabio Galuppo  
//March 2016 

#ifndef _accumulate_sample_
#define _accumulate_sample_

#include <array>
#include <list>
#include <numeric>
#include <algorithm>
#include <utility>
#include "my_swap_sample.hpp" //iota_random

template<typename T> using List = std::list<T>; //templatized type alias (http://www.urbandictionary.com/define.php?term=templatized)

template<typename T>
inline List<T>& my_push_front(List<T>& lst, const T& x)
{
	lst.push_front(x);
	return lst;
}

int accumulate_sample_main()
{
	using std::cout;

	using Int = unsigned short; //type alias
	std::array<Int, 10> xs;
	randomizer<Int> rnd;
	std::iota(xs.begin(), xs.end(), iota_random<Int>(rnd));
	std::for_each(xs.begin(), xs.end(), [](Int x) { cout << x << " "; });

	//http://simplycpp.com/2015/12/08/por-quem-os-ponteiros-dobram-estrelando-stdaccumulate/
	//sum
	unsigned long initial = 0;
	unsigned long total = std::accumulate(xs.begin(), xs.end(), initial); //implict operator+
	cout << "\n" << total << "\n";
	
	//append	
	List<char> ys{ 'a', 'b', 'c' }, zs{ 'd' };
	cout << "before append:";
	std::for_each(zs.begin(), zs.end(), [](char z) { cout << z << " "; });
	cout << "\n";
	zs = std::accumulate(ys.rbegin(), ys.rend(), std::move(zs),
		[](List<char>& acc, char x) -> List<char>& {
			return my_push_front(acc, x);
		});
	cout << "after  append:";
	std::for_each(zs.begin(), zs.end(), [](char z) { cout << z << " "; });
	cout << "\n";

	return 0;
}

#endif /* _accumulate_sample_ */