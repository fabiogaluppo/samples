//Sample provided by Fabio Galuppo 
//August 2015

#ifndef _measure_stats_
#define _measure_stats_

#include "measure.hpp"

#include <vector>
#include <tuple>
#include <cmath>
#include <limits>
#include <numeric>
#include <iostream>
#include <iomanip>

using stats_item = std::tuple<double /* N */, double /* elapsed */>;
using stats_table = std::vector<stats_item>;

double display_stats(const stats_table& stats)
{
	double order_of_growth = std::get<0>(stats[1]) / std::get<0>(stats[0]);

	auto log = [](double value, double base) -> double { return std::log(value) / std::log(base); };

	std::cout << std::setw(10) << "N" << std::setw(16) << "T" << std::setw(10) << "log N" << std::setw(10) << "log T" << std::setw(10) << "ratio" << std::setw(10) << "log ratio" << "\n";

	stats_table log_stats;
	for (size_t i = 0; i < stats.size(); ++i)
	{
		stats_item& x = const_cast<stats_item&>(stats[i]);
		log_stats.push_back(std::make_tuple(log(std::get<0>(x), order_of_growth), log(std::get<1>(x), order_of_growth)));
	}

	stats_table ratio_stats;
	for (size_t i = stats.size() - 1; i > 0; --i)
	{
		double ratio = std::get<1>(stats[i]) / std::get<1>(stats[i - 1]);
		double log_ratio = log(ratio, order_of_growth);
		ratio_stats.push_back(std::make_tuple(ratio, log_ratio));
	}
	ratio_stats.push_back(std::make_tuple(-std::numeric_limits<double>::infinity(), -std::numeric_limits<double>::infinity()));
	std::reverse(ratio_stats.begin(), ratio_stats.end());

	for (size_t i = 0; i < stats.size(); ++i)
	{
		stats_item& x = const_cast<stats_item&>(stats[i]);
		stats_item& y = log_stats[i];
		stats_item& z = ratio_stats[i];
		std::cout << std::fixed << std::setprecision(4) << std::setw(10) << static_cast<size_t>(std::get<0>(x)) << std::setw(16)
			<< std::get<1>(x) << std::setw(10) << static_cast<size_t>(std::get<0>(y)) << std::setw(10)
			<< std::get<1>(y) << std::setw(10) << std::get<0>(z) << std::setw(10) << std::get<1>(z) << "\n";
	}

	return std::get<1>(*(ratio_stats.end() - 1));
}

template<class ElapsedPolicy>
void display_estimated_runnning_time(const stats_table& stats, double N, double b)
{
	double t = std::accumulate(stats.cbegin(), stats.cend(), 0.0,
		[](double acc, const stats_item& x){ return acc + std::get<1>(x); }) / stats.size();

	double a = t / std::pow(N, b);

	ElapsedPolicy ep;
	std::cout << "Estimated runnning time is " << std::scientific << a << " x N^" <<
		std::fixed << std::setprecision(4) << b << " " << ep.sym() << "\n";
}

template<class ElapsedPolicy, template <typename> class TestFunctor>
void do_measurement_and_stats(std::initializer_list<size_t> Ns)
{
	TestFunctor<ElapsedPolicy> f;
	stats_table results = f(Ns);
	double b = display_stats(results);
	const size_t n = *(Ns.begin() + (Ns.size() - 1));
	display_estimated_runnning_time<ElapsedPolicy>(f({ n, n, n, n, n }), n, b);
}

#endif /* _measure_stats_ */
