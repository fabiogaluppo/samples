//Sample provided by Fabio Galuppo
//May 2014

#pragma once
#ifndef _merge_sort_sample_
#define _merge_sort_sample_

#include <iterator>
#include <iostream>
#include <vector>
#include <thread>

template <class RandomAccessIterator>
void display(RandomAccessIterator first, RandomAccessIterator end)
{
	for (RandomAccessIterator it = first; it != end; ++it) 
		std::cout << *it << " ";
	std::cout << std::endl;
}

template <class RandomAccessIterator>
void merge(RandomAccessIterator first, RandomAccessIterator end, typename RandomAccessIterator::difference_type mid)
{
	std::vector<RandomAccessIterator::value_type> aux(first, end);
	
	RandomAccessIterator M = begin(aux) + mid, E = std::end(aux);
	RandomAccessIterator i = begin(aux), j = M;
	
	for (RandomAccessIterator it = first; it != end; ++it)
	{
			 if (i >= M)  { *it = *j; ++j; }
		else if (j >= E)  { *it = *i; ++i; }
		else if (*i < *j) { *it = *i; ++i; }
		else              { *it = *j; ++j; }
	}
}

template <class RandomAccessIterator>
void merge_sort(RandomAccessIterator first, RandomAccessIterator end)
{
	if (end - first <= 1) return;

	auto mid = std::distance(first, end) / 2;
	merge_sort(first, first + mid);
	merge_sort(first + mid, end);
	merge(first, end, mid);
}

#include <sstream>

template <class RandomAccessIterator>
void parallel_merge_sort(RandomAccessIterator first, RandomAccessIterator end)
{
	if (end - first <= 1) return;

	auto size = std::distance(first, end);
	auto nothreads = std::thread::hardware_concurrency();

	std::vector<std::thread> threads;
	auto it = first;
	
	auto f = [](RandomAccessIterator f, RandomAccessIterator e) 
	{ 
		std::stringstream ss;
		ss << std::distance(f, e) << " " << &f << " " << &e << std::endl;
		std::cout << ss.str();
		merge_sort(f, e); 
	};

	auto stride = static_cast<unsigned>(size * (1.0 / nothreads));
	for (unsigned i = 1; i < nothreads; ++i, it += stride)
	{
		std::thread t(f, it, it + stride);
		threads.push_back(std::move(t));
	}
	std::thread t(f, it, end);
	threads.push_back(std::move(t));
	for (auto& th : threads)
		th.join();

	merge(first, end, std::distance(first, end) / 2);
}

#include <random>
#include <ctime>
#include <chrono>

void run_merge_sort_sample()
{
	//std::vector<int> xs{ 5, 3, 1, 8, 2, 19, 10, 6, 200, 85 };

	std::default_random_engine engine(static_cast<unsigned>(std::time(nullptr)));
	std::uniform_int_distribution<int> rnd(1, 1000000);
	std::vector<int> xs;
	xs.reserve(100000000);
	//xs.reserve(1000000);
	//xs.reserve(100);
	
	auto i = xs.capacity();
	while (i-- > 0)
		xs.push_back(rnd(engine));
	
	std::cout << "container size = " << xs.size() << std::endl;

	//display(std::begin(xs), std::end(xs));
	display(std::begin(xs), std::begin(xs) + 10);
	display(std::end(xs) - 10, std::end(xs));

	auto start = std::chrono::high_resolution_clock::now();
	
	//merge_sort(std::begin(xs), std::end(xs));
	parallel_merge_sort(std::begin(xs), std::end(xs));
	
	auto elapsed = std::chrono::high_resolution_clock::now() - start;
	
	display(std::begin(xs), std::begin(xs) + 10);
	display(std::end(xs) - 10, std::end(xs));
	//display(std::begin(xs), std::end(xs));

	std::cout << std::chrono::duration_cast<std::chrono::milliseconds>(elapsed).count() << " ms" << std::endl;	
}

#endif /* _merge_sort_sample_ */