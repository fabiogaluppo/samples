//Sample provided by Fabio Galuppo 
//August 2015

//compile: cl.exe /Fo.\bin\obj\ /EHsc /Ox program.cpp /link /out:bin\program.exe
//run: .\bin\program.exe

#include "interval.hpp"

#include <iostream>
#include <iomanip>
#include <thread>

void Test_interval()
{
	//time point
    /*
    auto tp = std::chrono::high_resolution_clock::now();
	auto c = tp.time_since_epoch().count();
	std::chrono::duration<long long> x(c);
	std::chrono::time_point<std::chrono::high_resolution_clock, std::chrono::duration<long long>> x2(x);
	*/
    
	time_line_segment_1D_collection xs;	
    time_ruler m1, m2, m3;
	std::this_thread::sleep_for(std::chrono::seconds(1));
	xs.collect(m2.get());
	std::this_thread::sleep_for(std::chrono::seconds(2));
	xs.collect(m3.get());
	std::this_thread::sleep_for(std::chrono::seconds(1));
	xs.collect(m1.get());
	std::this_thread::sleep_for(std::chrono::seconds(1));
	time_ruler m4;
	std::this_thread::sleep_for(std::chrono::seconds(2));
	time_ruler m5;
	std::this_thread::sleep_for(std::chrono::seconds(1));
	xs.collect(m4.get());
	//std::this_thread::sleep_for(std::chrono::milliseconds(100));
	xs.collect(m5.get());
	std::this_thread::sleep_for(std::chrono::seconds(3));
	time_ruler m6;
	std::this_thread::sleep_for(std::chrono::milliseconds(500));
	xs.collect(m6.get());

	auto result = xs.compute();
	//how to use duration_cast:
    /*
    auto avg = result.average();
    std::chrono::duration<double> d(avg);
	auto s = std::chrono::duration_cast<std::chrono::milliseconds>(d).count();
	std::cout << s << "\n";
    */

	auto minmax = result.minmax();

	std::cout 
		<< "Count   = " << std::setw(6) << result.count() << "\n"
		<< "Total   = " << std::setw(6) << std::chrono::duration_cast<std::chrono::milliseconds>(result.total()).count() << " ms" << "\n"
		<< "Average = " << std::setw(6) << std::chrono::duration_cast<std::chrono::milliseconds>(result.average()).count() << " ms" << "\n"
		<< "Std Dev = " << std::setw(6) << std::chrono::duration_cast<std::chrono::milliseconds>(result.stddev()).count() << " ms" << "\n"
		<< "Min     = " << std::setw(6) << std::chrono::duration_cast<std::chrono::milliseconds>(std::get<0>(minmax).size()).count() << " ms" << "\n"
		<< "Max     = " << std::setw(6) << std::chrono::duration_cast<std::chrono::milliseconds>(std::get<1>(minmax).size()).count() << " ms" << "\n";
}

int main()
{
	Test_interval();

	return 0;
}