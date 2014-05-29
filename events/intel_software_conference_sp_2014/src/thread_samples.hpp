//Sample provided by Fabio Galuppo
//May 2014

#pragma once
#ifndef _thread_samples_
#define _thread_samples_

#include <thread>

#include <iostream>
#include <sstream>

void thread_func()
{
	std::stringstream ss;
	ss << "inside thread with id "<< std::this_thread::get_id() << "..." << std::endl;
	std::cout << ss.str();
}

struct thread_functor final
{
	thread_functor() {}

	void operator()() const
	{
		thread_func();
	}

	thread_functor(thread_functor&&) {}
	//thread_functor(const thread_functor&) = default;
	
	thread_functor(const thread_functor&) = delete;
	thread_functor& operator=(const thread_functor&) = delete;
};

#include <exception>

struct thread_functor2 final
{
	thread_functor2() {}

	void operator()(bool throws) const
	{
		if (throws) throw std::exception("an exception happened...");
		thread_func();
	}
};

#include <system_error>
#include <chrono>

struct scoped_thread final
{
	explicit scoped_thread(std::thread t) 
		: T_(std::move(t))
	{
		if (!T_.joinable())		
			throw std::invalid_argument("The thread object is not joinable");
	}

	scoped_thread(scoped_thread&& that) //move constructor
	{
		T_.swap(that.T_);
		that.Moving_ = true;
	}

	scoped_thread& operator=(scoped_thread&& that) //move assignment
	{
		if (this != &that)
		{
			T_ = std::move(that.T_);
			that.Moving_ = true;
		}

		return *this;
	}

	~scoped_thread() { if (!Moving_)  T_.join(); }

	scoped_thread(const scoped_thread&) = delete;
	scoped_thread& operator=(const scoped_thread&) = delete;

private:
	std::thread T_;
	bool Moving_ = false;
};

template<class _Fn, class... _Args>
inline scoped_thread make_scoped_thread(_Fn f, _Args... args)
{
	return std::move(scoped_thread(std::thread{ f, args... }));
}

void run_thread_samples()
{
	{
		std::thread t1(thread_func);
		std::thread t2{ thread_functor() };

		t1.join();

		t2.join();
		//if (t2.joinable()) t2.detach();
		try
		{
			t2.join();
		}
		catch (std::system_error& e)
		{
			auto err = e.code();
			if (err.value() == (int)std::errc::invalid_argument)
				std::cout << "The thread object is not joinable" << std::endl;
			else
				std::cout << e.what() << std::endl;
		}
	}

	{
		//std::thread t1{ thread_functor2(), true };
		std::thread t1{ thread_functor2(), false };
		
		std::thread t2([](){ 

			std::this_thread::sleep_for(std::chrono::seconds(2));
			
			std::stringstream ss;
			ss << "inside thread with id " << std::this_thread::get_id() << "..." << std::endl;
			std::cout << ss.str();

		}); //with lambda expression
		std::thread t3 = std::move(t2); //transferring ownership
		t3.join();
		
		try
		{
			t1.join();
		}
		catch (...) //there's no exception propagation!
		{
			std::cout << "error..." << std::endl;
		}
	}

	{
		auto f = [](const char* msg, int sec) { 
			std::this_thread::sleep_for(std::chrono::seconds(sec));
			std::stringstream ss;
			ss << msg << " inside thread with id " << std::this_thread::get_id() << "..." << std::endl;
			std::cout << ss.str();
		};

		scoped_thread st(std::thread(f, "scoped_thread in action", 1));
		auto st2{ make_scoped_thread(f, "make_scoped_thread in action", 1) };
	}
}

#endif /* _thread_samples_ */