//Sample provided by Fabio Galuppo
//May 2013

#include "tsp.hpp"
#include "simulated_annealing.hpp"

#include <utility>
#include <random>
#include <memory>
#include <ctime>

template<int N>
auto setup_environment(tsp_class<N>& tsp_instance, std::function<int(void)> rnd_location) -> void
{
	for (int i = 0; i < N; ++i) 
		tsp_instance.cities[i] = std::make_pair(rnd_location(), rnd_location());
}

struct random_functor
{
	random_functor(int min_inclusive, int max_inclusive) : 
		Engine_(new std::default_random_engine(static_cast<unsigned>(std::time(nullptr)))),
		Rnd_(new std::uniform_int_distribution<int>(min_inclusive, max_inclusive))
	{
	}

	random_functor(int max_exclusive) :
		Engine_(new std::default_random_engine(static_cast<unsigned>(std::time(nullptr)))),
		Rnd_(new std::uniform_int_distribution<int>(0, max_exclusive - 1))
	{
	}

	//[0; max_inclusive]
	auto operator()() const -> int { return (*Rnd_)(*Engine_); }

private:
	std::shared_ptr<std::default_random_engine> Engine_;
	std::shared_ptr<std::uniform_int_distribution<int>> Rnd_;
};

struct double_random_functor
{
	double_random_functor() : 
		Engine_(new std::default_random_engine(static_cast<unsigned>(std::time(nullptr)))), 
	    Rnd_(new std::uniform_real_distribution<double>())
	{
	}

	//[0.0; 1.0]
	auto operator()() const -> double  { return (*Rnd_)(*Engine_); }

private:
	std::shared_ptr<std::default_random_engine> Engine_;
	std::shared_ptr<std::uniform_real_distribution<double>> Rnd_;
};

#include <thread>
#include <future>
#include <chrono>
#include <vector>
#include <algorithm>
#include <iostream>

struct stop_watch
{
	stop_watch() : 
		Start_(now()) 
	{
	}

	auto elapsed_ms() const -> std::chrono::milliseconds
	{
		return std::chrono::duration_cast<std::chrono::milliseconds>(now() - Start_);
	}

	void restart() { Start_ = now(); }

private:
	static auto now() -> std::chrono::high_resolution_clock::time_point
	{ 
		return std::chrono::high_resolution_clock::now(); 
	}

	std::chrono::high_resolution_clock::time_point Start_;
};

struct random_delayer
{
	random_delayer(int min_inclusive, int max_inclusive) : 
		Delayer_(min_inclusive, max_inclusive) {}

	auto delay_seconds() const -> int 
	{ 
		int delay = Delayer_();
		std::this_thread::sleep_for(std::chrono::seconds(delay));
		return delay;
	}

private:
	random_functor Delayer_;
};

template <int N>
auto multi_threaded_simulated_annealing(tsp_class<N>& tsp_instance, const int number_of_tasks_to_spawn) -> void
{
	stop_watch sw;
	
	std::cout << "START SOLUTION:" << std::endl;
	display(tsp_instance, true);
	
	const int NUMBER_OF_TASKS_TO_SPAWN = number_of_tasks_to_spawn;	
	std::vector<std::future<tsp_class<N>>> tsp_solutions;

	random_delayer delayer(1, 3);
	for(int i = 0; i < NUMBER_OF_TASKS_TO_SPAWN; ++i)
	{
		delayer.delay_seconds();

		tsp_solutions.push_back( std::async([tsp_instance] {
			const bool MIN_CYCLE = true;
			const bool MIN_CYCLE_KEEPING_START = false;

			auto tsp_solution = tsp_instance;
			simulated_annealing(100.0, 0.0001, 0.999, 200, MIN_CYCLE_KEEPING_START, tsp_solution, 
				random_functor(N), double_random_functor());
			return tsp_solution;
		}));
	}
	
	std::cout << "CANDIDATE SOLUTIONS:" << std::endl;
	std::vector<tsp_class<N>> candidate_solutions; 
	for (auto& tsp_solution : tsp_solutions)
	{
		auto candidate = tsp_solution.get();
		candidate_solutions.push_back(candidate);
		display(candidate, false);
	}

	std::cout << "CANDIDATE LENGTHS:" << std::endl;
	for (auto& solution : candidate_solutions)
		std::cout << solution.do_cycle_lenght() << std::endl;

	std::sort( candidate_solutions.begin(), candidate_solutions.end(), 
		[](const tsp_class<N>& left, const tsp_class<N>& right) { 
			return left.do_cycle_lenght() < right.do_cycle_lenght(); });

	std::cout << "CANDIDATE LENGTHS (ORDERED):" << std::endl;
	for (auto& solution : candidate_solutions)
		std::cout << solution.do_cycle_lenght() << std::endl;

	std::cout << "SELECTED SOLUTION:" << std::endl;
	display(*candidate_solutions.begin(), true);

	std::cout << sw.elapsed_ms().count() << " ms";
}

#include <fstream>
#include <cstdlib>

auto display_usage(const char* exe_name) -> void
{
	std::cout << "To generate a TSP random file:\n";
	std::cout << "  " << exe_name << " " << "filename:string\n";
	std::cout << "\nTo read a TSP random file and execute:\n";
	std::cout << "  " << exe_name << " " << "filename:string " << "k_tasks:int";
}

auto main(int argc, const char* argv[]) -> int
{
	const int N = 500;
	if (argc == 2)
	{
		//generate random tsp file
		tsp_class<N> tsp_instance;
		setup_environment(tsp_instance, random_functor(1, 100));

		std::ofstream ofs (argv[1], std::ofstream::out | std::ofstream::binary);
		tsp_instance.save(ofs);
		ofs.close();
	}
	else if (argc == 3)
	{
		//read random tsp file
		const char* filename = argv[1];
		std::ifstream ifs (filename, std::ofstream::in | std::ofstream::binary);
		tsp_class<N> tsp_instance(ifs);
		ifs.close();

		//execute simulated annelaling with k tasks
		int k = std::atoi(argv[2]);
		multi_threaded_simulated_annealing<N>(tsp_instance, k);
	}
	else
	{
		display_usage(argv[0]);
	}

	return 0;
}