//Sample provided by Fabio Galuppo
//May 2014

#pragma once
#ifndef _future_sample_
#define _future_sample_

#include <thread>
#include <future>
#include <chrono>

#include <iostream>

#include <exception>
#include <stdexcept> 

void run_future_sample()
{
	//future and async
	std::future<int> f =
		std::async([]() -> int {
		std::this_thread::sleep_for(std::chrono::seconds(2));
		return 100;
	});

	std::cout << f.get() << std::endl;

	auto g = [](std::promise<int>& p, int value) {
		if (value <= 0)
		{
			auto e = std::make_exception_ptr(std::runtime_error("value less than or equal to 0"));
			p.set_exception(e);
			return;
		}

		std::this_thread::sleep_for(std::chrono::seconds(2));
		p.set_value(value * 100);
	};

	//promise
	std::promise<int> p1, p2;
	std::thread t1(g, std::ref(p1), 100);
	std::thread t2(g, std::ref(p2), -1);

	auto f1 = p1.get_future();
	auto f2 = p2.get_future();

	f1.wait();
	f2.wait();

	std::cout << f1.get() << std::endl;
	try
	{
		std::cout << f2.get() << std::endl;
	}
	catch (const std::exception& e)
	{
		std::cout << "exception caught: " << e.what() << std::endl;
	}

	t1.join();
	t2.join();

	//packaged_task
	auto h = [](int a, int b)
	{
		std::this_thread::sleep_for(std::chrono::seconds(2));
		int result = a * b;
		if (result <= 0)
			throw std::runtime_error("result less than or equal to 0");
		return result;
	};
	std::packaged_task<int(int, int)> pt1(h);
	std::future<int> f3 = pt1.get_future();
	std::thread t3(std::move(pt1), 10, 90);

	std::packaged_task<int(int, int)> pt2(h);
	std::future<int> f4 = pt2.get_future();
	std::thread t4(std::move(pt2), 10, -90);
	t3.detach();
	t4.detach();

	std::cout << f3.get() << std::endl;
	try
	{
		std::cout << f4.get() << std::endl;
	}
	catch (const std::exception& e)
	{
		std::cout << "exception caught: " << e.what() << std::endl;
	}
}

#endif /* _future_sample_ */