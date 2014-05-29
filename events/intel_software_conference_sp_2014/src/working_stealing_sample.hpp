//Sample provided by Fabio Galuppo
//May 2014

#pragma once
#ifndef _working_stealing_sample_
#define _working_stealing_sample_

#include <mutex>
#include <condition_variable>
#include <future>
#include <chrono>
#include <deque>

template <typename T>
struct semi_deque
{
	void push(T item)
	{
		std::lock_guard<std::mutex> lock(CR_);
		Q_.push_back(item);
		NonEmpty_.notify_all();
	}

	bool try_pop_front(T& outVal)
	{
		std::unique_lock<std::mutex> lock(CR_);		
		if (NonEmpty_.wait_for(lock, TimeOut_, [this](){ return !Q_.empty(); }))
		{
			outVal = Q_.front();
			Q_.pop_front();
			return true;
		}
		
		return false;
	}

	bool try_pop_back(T& outVal)
	{
		std::unique_lock<std::mutex> lock(CR_);
		if (NonEmpty_.wait_for(lock, TimeOut_, [this](){ return !Q_.empty() && Q_.size() > 1; }))
		{
			outVal = Q_.back();
			Q_.pop_back();
			return true;
		}

		return false;
	}

	typename std::deque<T>::size_type size() 
	{
		std::lock_guard<std::mutex> lock(CR_);
		return Q_.size();
	}

private:
	std::condition_variable NonEmpty_;
	std::mutex CR_;
	std::deque<T> Q_;
	std::chrono::milliseconds TimeOut_ = std::chrono::milliseconds(200);
};

#include <functional>
#include <memory>
#include <iostream>
#include <string>
#include <random>
#include <ctime>

void pause(int sec)
{
	std::this_thread::sleep_for(std::chrono::seconds(sec));
}

void run_working_stealing_sample()
{
	using task_t = std::packaged_task<void(int, bool)>;
	using semi_deque_t = semi_deque<std::shared_ptr<task_t>>;

	std::function<void(int, int, bool)> f = [](int sec, int id, bool stolen){
		std::string s = "thread ";
		s += std::to_string(id);
		s += " waiting for ";
		s += std::to_string(sec);
		s += " second(s)\r\n";
		if (stolen) s = "* " + s;
		std::cout << s;

		pause(sec);
	};

	using namespace std::placeholders;

	semi_deque_t q1, q2;
	for (int i = 1; i <= 10; ++i)
		q1.push(std::make_shared<task_t>(std::bind(f, i, _1, _2)));
	for (int i = 1; i <= 2; ++i)
		q2.push(std::make_shared<task_t>(std::bind(f, i, _1, _2)));

	auto g = [](int id, semi_deque_t* this_q, semi_deque_t* that_q){
		std::shared_ptr<task_t> t;
		while (true)
		{
			if (this_q->try_pop_front(t))
			{
				(*t)(id, false);
				continue;
			}

			if (that_q->try_pop_back(t))
			{
				(*t)(id, true);				
			}
		}
	};

	std::thread ws_t1(g, 0, &q1, &q2);
	std::thread ws_t2(g, 1, &q2, &q1);

	std::default_random_engine engine(static_cast<unsigned>(std::time(nullptr)));
	std::uniform_int_distribution<int> rnd(1, 6);

	pause(30);

	while (true)
	{
		bool can_pause = false;
		if (0x1 == (rnd(engine) & 0x1))
		{
			if (q1.size() == 0)
			{
				for (int i = 1, l = rnd(engine); i <= l; ++i)
					q1.push(std::make_shared<task_t>(std::bind(f, rnd(engine), _1, _2)));
				can_pause = true;
			}
		}
		else
		{
			if (q2.size() == 0)
			{
				for (int i = 1, l = rnd(engine); i <= l; ++i)
					q2.push(std::make_shared<task_t>(std::bind(f, rnd(engine), _1, _2)));
				can_pause = true;
			}
		}
		
		if (can_pause) pause(10);
	}

	ws_t1.join();
	ws_t2.join();
}

#endif /* _working_stealing_sample_ */