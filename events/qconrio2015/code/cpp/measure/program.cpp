//Sample provided by Fabio Galuppo 
//August 2015

//compile: cl.exe /Fo.\bin\obj\ /EHsc /Ox program.cpp /link /out:bin\program.exe
//run: .\bin\program.exe

#include "measure_stats.hpp"

#include "merge_sample.hpp"

#include <iostream>
#include <iomanip>
#include <algorithm>
#include <random>
#include <numeric>

template<class ElapsedPolicy>
stats_table Test_max_element(std::initializer_list<size_t> Ns)
{
	stats_table results;

	for (size_t N : Ns)
	{
		std::vector<int> xs;
		xs.resize(N);
		std::iota(xs.begin(), xs.end(), 1);

		unsigned seed = 1234567890;
		std::shuffle(xs.begin(), xs.end(), std::default_random_engine(seed));

		double elapsed{};
		int result{};
		auto m = do_measurement<ElapsedPolicy>([&]() {

			const auto& max_iter = std::max_element(xs.begin(), xs.end());
			result = *max_iter;

		}, "max_element algorithm", elapsed);

		std::cout << "Elapsed time: " << m << " N: " << std::setw(8) << N
			<< " result: " << std::setw(8) << result << ".\n";

		results.push_back(std::make_tuple(static_cast<double>(N), elapsed));
	}

	return results;
}

template<class ElapsedPolicy>
void Test_k_way_merge_sort(std::initializer_list<size_t> Ns, std::initializer_list<size_t> Ks)
{
	for (size_t K : Ks)
	for (size_t N : Ns)
	{
		std::vector<int> xs, ys;
		xs.resize(N);
		ys.resize(N);

		std::iota(xs.begin(), xs.end(), 1);
		unsigned seed = 1234567890;
		std::shuffle(xs.begin(), xs.end(), std::default_random_engine(seed));
		std::copy(xs.begin(), xs.end(), ys.begin());

		{
			double elapsed{};
			int result{};
			auto m = do_measurement<ElapsedPolicy>([&]() {

				k_way_merge_sort_1(xs, K);
				result = xs.size();

			}, "k_way_merge_sort_1", elapsed);

			std::cout << "Elapsed time = " << m << " N = " << std::setw(8) << N << " K = " << std::setw(2) << K << " : " << std::setw(8) << result << ".\n";
		}
		{
			double elapsed{};
			int result{};
			auto m = do_measurement<ElapsedPolicy>([&]() {

				k_way_merge_sort_2(ys, K);
				result = ys.size();

			}, "k_way_merge_sort_2", elapsed);

			std::cout << "Elapsed time = " << m << " N = " << std::setw(8) << N << " K = " << std::setw(2) << K << " : " << std::setw(8) << result << ".\n";
		}

		std::cout << std::string(64, '-') << "\n";
	}
}

template<class ElapsedPolicy>
struct Test_max_element_functor
{
	stats_table operator()(std::initializer_list<size_t> Ns)
	{
		return Test_max_element<ElapsedPolicy>(Ns);
	}
};


int main()
{
	const std::size_t MB = 1024 * 1024;
    std::initializer_list<size_t> Ns{ MB * 1, MB * 2, MB * 4, MB * 8, MB * 16, MB * 32, MB * 64 };
    
    //Test 1:
    std::initializer_list<size_t> Ks{ 1, 2, 4, 8, 16, 32, 64 };	
	Test_k_way_merge_sort<ElapsedMilliseconds>(Ns, Ks);
    
	//Test 2:
    //stats_table results = Test_max_element<ElapsedMilliseconds>(Ns);
	//double b = display_stats(results);    

	//Test 3:
    /*
    stats_table xs{ std::make_tuple(200, 0.1), std::make_tuple(400, 0.1), 
					std::make_tuple(800, 0.2), std::make_tuple(1600, 0.9), 
					std::make_tuple(3200, 7.5), std::make_tuple(6400, 62.2), 
					std::make_tuple(12800, 516.7) };
	display_stats(xs);
    */
    
	//Test 4:
    //do_measurement_and_stats<ElapsedMilliseconds, Test_max_element_functor>(Ns);

	return 0;
}