//Sample provided by Fabio Galuppo
//Compositional TSP Solver version 0.1
//October 2013

#pragma once
#ifndef _tsp_functors_
#define _tsp_functors_

#include "tsp.hpp"
#include "tsp_monad.hpp"

TSP Propagator(const TSP& oldValue, const TSP& newValue, bool propagate_inferior_results = false)
{
	//accept inferior results (if it's an inferior result!)
	if (propagate_inferior_results)	
		return newValue;
	
	//do not propagate inferior results
	if (ref(oldValue).do_cycle_length() > ref(newValue).do_cycle_length())
		return newValue;

	return oldValue;
};

enum base_args_id
{
	FORK_JOIN_ARGS_ID = 1
};

struct Chain
{
	typedef TSP::transformer_type F;

	Chain(F f0)
	{
		Funcs_.push_back(f0);
	}

	Chain(F f0, F f1)
	{
		Funcs_.push_back(f0);
		Funcs_.push_back(f1);
	}

	Chain(F f0, F f1, F f2)
	{
		Funcs_.push_back(f0);
		Funcs_.push_back(f1);
		Funcs_.push_back(f2);
	}

	Chain(F f0, F f1, F f2, F f3)
	{
		Funcs_.push_back(f0);
		Funcs_.push_back(f1);
		Funcs_.push_back(f2);
		Funcs_.push_back(f3);
	}

	Chain(F f0, F f1, F f2, F f3, F f4)
	{
		Funcs_.push_back(f0);
		Funcs_.push_back(f1);
		Funcs_.push_back(f2);
		Funcs_.push_back(f3);
		Funcs_.push_back(f4);
	}

	Chain(F f0, F f1, F f2, F f3, F f4, F f5)
	{
		Funcs_.push_back(f0);
		Funcs_.push_back(f1);
		Funcs_.push_back(f2);
		Funcs_.push_back(f3);
		Funcs_.push_back(f4);
		Funcs_.push_back(f5);
	}

	//...
	
	TSP operator()(TSP::T t)
	{
		for(auto f = Funcs_.cbegin(); f != Funcs_.cend(); ++f)
			(*f)(t);
		return TSP(t);
	}

private:
	std::vector<F> Funcs_;
};

struct ForkJoin_args : public base_args
{
	ForkJoin_args(unsigned int i, unsigned int number_of_tasks_in_parallel) : 
		base_args(FORK_JOIN_ARGS_ID), 
		i(i), 
		number_of_tasks_in_parallel(number_of_tasks_in_parallel) {}
	const unsigned int i;
	const unsigned int number_of_tasks_in_parallel;
};

#include <future>
#include <memory>

struct ForkJoin
{
	typedef TSP::transformer_type F;

	ForkJoin(unsigned int number_of_tasks_in_parallel, F f, bool propagate_inferior_results = false) : 
		NumberOfTasksInParallel_(number_of_tasks_in_parallel),
		Func_(f),
		PropagateInferiorResults_(propagate_inferior_results)
	{
	}

	TSP operator()(TSP::T t)
	{
		if (0 == NumberOfTasksInParallel_)
			return TSP(t);
		
		std::vector<std::future<tsp_async_state>> tsp_solutions;

		std::cout << "START SOLUTION:" << std::endl;
		
		auto tsp_instance = ref(t);
		display_solution(tsp_instance);

		for (unsigned int i = 0; i < NumberOfTasksInParallel_; ++i)
		{
			//async
			tsp_solutions.push_back(std::async(std::launch::async, [&, i, tsp_instance] 
			{
				Maybe a = just(tsp_instance);
				std::unique_ptr<ForkJoin_args> args(new ForkJoin_args(i, NumberOfTasksInParallel_));
				a.second = args.get();
				return tsp_async_state(Func_(a));
			}));
		}

		std::cout << "CANDIDATE SOLUTIONS:" << std::endl;
		std::vector<tsp_class> candidate_solutions;
		int i = 0;
		for (auto& tsp_solution : tsp_solutions)
		{
			//async result
			auto candidate = tsp_solution.get();
			if (has(candidate.state))
			{
				candidate_solutions.push_back(ref(candidate.state));
				display(ref(candidate.state), false);
			}
		}

		std::cout << "CANDIDATE LENGTHS:" << std::endl;
		for (auto& solution : candidate_solutions)
			std::cout << solution.do_cycle_length() << std::endl;

		std::sort(candidate_solutions.begin(), candidate_solutions.end(), 
			[](const tsp_class& left, const tsp_class& right) 
				{ return left.do_cycle_length() < right.do_cycle_length(); });

		std::cout << "CANDIDATE LENGTHS (ORDERED):" << std::endl;
		for (auto& solution : candidate_solutions)
			std::cout << solution.do_cycle_length() << std::endl;

		std::cout << "SELECTED SOLUTION:" << std::endl;
		auto result = Propagator(TSP(t), TSP(just(*candidate_solutions.begin())), PropagateInferiorResults_);
		display_solution(ref(result));
		return result;
	}

private:
	unsigned int NumberOfTasksInParallel_;
	F Func_;
	bool PropagateInferiorResults_;

	static void display_solution(tsp_class& tsp_instance)
	{
		display(tsp_instance, true);
		std::cout << tsp_instance.do_cycle_length() << std::endl;
	}

	struct tsp_async_state
	{
		tsp_async_state() 
			: state(TSP(nothing())) {}
		explicit tsp_async_state(const TSP& state) 
			: state(state) {}
		TSP state;
	};
};

struct Generations
{
	typedef TSP::transformer_type F;

	Generations(unsigned int number_of_generations, F f) : 
		NumberOfGenerations_(number_of_generations),
		Func_(f)
	{
	}

	TSP operator()(TSP::T t)
	{
		TSP::T result = t; 
		for(unsigned int g = 0; g < NumberOfGenerations_; ++g) 
		{
			std::cout << "GENERATION:" << g + 1 << std::endl;
			auto new_result = Func_(result);
			result = just(new_result);
		}
		return TSP(result);
	}

private:
	unsigned int NumberOfGenerations_;
	F Func_;
};

struct Identity
{
	TSP operator()(TSP::T t)
	{
		return TSP(t);
	}
};

#include <atomic>

struct Circular
{
	typedef TSP::transformer_type F;

	Circular(F f0) : 
		MaxCount_(1)
	{
		Counter_ = 0;
		Funcs_.push_back(f0);
	}

	Circular(F f0, F f1) :
		MaxCount_(2)
	{
		Counter_ = 0;
		Funcs_.push_back(f0);
		Funcs_.push_back(f1);
	}

	Circular(F f0, F f1, F f2) :
		MaxCount_(3)
	{
		Counter_ = 0;
		Funcs_.push_back(f0);
		Funcs_.push_back(f1);
		Funcs_.push_back(f2);
	}

	Circular(F f0, F f1, F f2, F f3) :
		MaxCount_(4)
	{
		Counter_ = 0;
		Funcs_.push_back(f0);
		Funcs_.push_back(f1);
		Funcs_.push_back(f2);
		Funcs_.push_back(f3);
	}

	Circular(F f0, F f1, F f2, F f3, F f4) :
		MaxCount_(5)
	{
		Counter_ = 0;
		Funcs_.push_back(f0);
		Funcs_.push_back(f1);
		Funcs_.push_back(f2);
		Funcs_.push_back(f3);
		Funcs_.push_back(f4);
	}

	Circular(F f0, F f1, F f2, F f3, F f4, F f5) :
		MaxCount_(6)
	{
		Counter_ = 0;
		Funcs_.push_back(f0);
		Funcs_.push_back(f1);
		Funcs_.push_back(f2);
		Funcs_.push_back(f3);
		Funcs_.push_back(f4);
		Funcs_.push_back(f5);
	}

	//...
	
	TSP operator()(TSP::T t)
	{
		Funcs_[Counter_++ % MaxCount_](t);
		return TSP(t);
	}

private:
	std::vector<F> Funcs_;
	std::atomic_int Counter_;
	const unsigned int MaxCount_;
};

#endif /* _tsp_functors_ */