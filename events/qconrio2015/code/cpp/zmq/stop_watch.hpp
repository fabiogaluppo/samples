//Sample provided by Fabio Galuppo 
//July 2015

#ifndef _stop_watch_
#define _stop_watch_

#include <chrono>

struct stop_watch final
{
	stop_watch() : Start_(now()) {}

	std::chrono::seconds elapsed_s() const
	{
		using std::chrono::seconds;
		return std::chrono::duration_cast<seconds>(elapsed());
	}

	std::chrono::milliseconds elapsed_ms() const
	{
		using std::chrono::milliseconds;
		return std::chrono::duration_cast<milliseconds>(elapsed());
	}

	std::chrono::microseconds elapsed_us() const
	{
		using std::chrono::microseconds;
		return std::chrono::duration_cast<microseconds>(elapsed());
	}

	std::chrono::nanoseconds elapsed_ns() const
	{
		using std::chrono::nanoseconds;
		return std::chrono::duration_cast<nanoseconds>(elapsed());
	}

	void restart() { Start_ = now(); }

	stop_watch(const stop_watch&) = delete;
	stop_watch& operator=(const stop_watch&) = delete;

private:
	static std::chrono::high_resolution_clock::time_point now()
	{
		return std::chrono::high_resolution_clock::now();
	}

	std::chrono::duration<double> elapsed() const
	{
		return now() - Start_;
	}

	std::chrono::high_resolution_clock::time_point Start_;
};

#endif /* _stop_watch_ */
