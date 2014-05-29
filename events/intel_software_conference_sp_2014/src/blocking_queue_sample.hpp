//Sample provided by Fabio Galuppo
//May 2014

#pragma once
#ifndef _blocking_queue_sample_
#define _blocking_queue_sample_

#include <mutex>
#include <condition_variable>
#include <queue>

template<typename T>
struct blocking_queue final
{
	void enqueue(const T& item)
	{
		std::lock_guard<std::mutex> lock(CR_);
		Q_.push(item);
		NonEmpty_.notify_one();
	}

	T dequeue()
	{
		std::unique_lock<std::mutex> lock(CR_);
		NonEmpty_.wait(lock, [this](){ return !Q_.empty(); });
		//or
		//while (Q_.empty()) NonEmpty_.wait(lock);
		T temp = Q_.front();
		Q_.pop();
		return temp;
	}

private:
	std::condition_variable NonEmpty_;
	std::mutex CR_;
	std::queue<T> Q_;
};

#include <iostream>

#include <random>
#include <ctime>
#include <chrono>

template<typename F>
struct task
{
	task(const F& f) 
		: F_(f)	{}

	void execute() { F_(); }

private:
	F F_;
};

#include <future>
#include <functional>
#include <string>

using fun_t = std::function<void()>;

task<fun_t> make_task(int sec, std::string msg)
{
	return task<fun_t>([sec, msg](){
		std::this_thread::sleep_for(std::chrono::seconds(sec));
		std::cout << msg << std::endl;
	});
}

void blocking_queue_sample_with_task()
{
	blocking_queue<task<fun_t>> q;

	auto deq_f = std::async([&q](){
		while (true)
		{
			task<fun_t> t = q.dequeue();
			t.execute();
		}
	});

	std::default_random_engine engine(static_cast<unsigned>(std::time(nullptr)));
	std::uniform_int_distribution<int> rnd(1, 5);
	for (int i = 0; i < 10; ++i)
	{
		std::string msg = "Waiting for ";
		int sec = rnd(engine);
		msg += std::to_string(sec);
		msg += " second(s)";
		q.enqueue(make_task(sec, msg));
	}

	deq_f.wait();
}

void run_blocking_queue_sample()
{
	blocking_queue_sample_with_task();
	return;
	
	blocking_queue<int> q;

	//consumer
	auto deq_f = std::async([&q](){
		int value;
		while ((value = q.dequeue()) != 0)
			std::cout << value << " ";
	});
		
	//producer
	std::default_random_engine engine(static_cast<unsigned>(std::time(nullptr)));
	std::uniform_int_distribution<int> rnd(1, 100000);

	for (int i = 0; i < 10; ++i)
	{
		for (int j = 0; j < 10; ++j) q.enqueue(rnd(engine));
		std::this_thread::sleep_for(std::chrono::seconds(1));
	}

	std::this_thread::sleep_for(std::chrono::seconds(3));
	q.enqueue(0);

	deq_f.wait();

	std::cout << std::endl;
}

#endif /* _blocking_queue_sample_ */