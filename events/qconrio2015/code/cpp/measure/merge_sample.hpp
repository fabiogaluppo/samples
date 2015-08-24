//Sample provided by Fabio Galuppo 
//August 2015

#ifndef _merge_sample_
#define _merge_sample_

#include <vector>
#include <algorithm>
#include <cmath>
#include <stdexcept>

template<typename T>
void k_partitions_sort(std::vector<T>& xs, size_t K)
{
	size_t N = xs.size();
	size_t P = N / K;
	for (size_t i = 0; i < N; i += P)
		std::sort(xs.begin() + i, xs.begin() + (i + P));
}

void k_way_requires(size_t N, size_t K)
{
	//I'm interested only in power of 2
	double lgN = std::log2(N);
	bool N_holds = N > K && 0.0 == lgN - static_cast<int>(lgN);
	double lgK = std::log2(K);
	bool K_holds = K > 0 && 0.0 == lgK - static_cast<int>(lgK);
	if (!N_holds) 
		throw std::runtime_error("N is invalid");
	if (!K_holds) 
		throw std::runtime_error("K is invalid");
}

template<typename T>
void k_way_merge_sort_1(std::vector<T>& xs, size_t K)
{
	size_t N = xs.size();
	if (N > 1)
	{
		k_way_requires(N, K); //this sample implementation requirements

		k_partitions_sort(xs, K); //INVARIANT: each partition must be sorted
		if (K == 1) 
			return;

		std::vector<T> aux;
		aux.resize(N);
		
		//O(P * K^2)
		size_t P = N / K;
		size_t x = P;
		for (size_t i = 1; i < K; ++i, x += P)
		{
			std::merge(xs.begin(), xs.begin() + x, xs.begin() + x, xs.begin() + (x + P), aux.begin());
			std::copy(aux.begin(), aux.begin() + (x + P), xs.begin());
		}
	}
}

template<typename T>
void k_way_merge_sort_2(std::vector<T>& xs, size_t K)
{
	size_t N = xs.size();
	if (N > 1)
	{
		k_way_requires(N, K); //this sample implementation requirements

		k_partitions_sort(xs, K); //INVARIANT: each partition must be sorted
		if (K == 1) 
			return;

		std::vector<int> aux;
		aux.resize(N);

		//O(N/K * lg K)
		for (int k = K; k > 1; k >>= 1)
		{
			size_t P = N / k;
			size_t x = 0, y = x + P, z = y + P;
			for (int i = 1; i < k; i <<= 1, x = z, y = x + P, z = y + P)
			{
				std::merge(xs.begin() + x, xs.begin() + y, xs.begin() + y, xs.begin() + z, aux.begin());
				std::copy(aux.begin(), aux.begin() + (P + P), xs.begin() + x);
			}
		}
	}
}

#endif /* _merge_sample_ */