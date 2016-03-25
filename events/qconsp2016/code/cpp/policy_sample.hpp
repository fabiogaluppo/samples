//Sample provided by Fabio Galuppo  
//March 2016 

#ifndef _policy_sample_
#define _policy_sample_

#include <mutex>

struct mutex_lock_policy
{
	mutex_lock_policy() = default;
	~mutex_lock_policy() = default;
	mutex_lock_policy(const mutex_lock_policy&) = delete;
	mutex_lock_policy& operator=(const mutex_lock_policy&) = delete;

	void lock()   { m.lock(); }
	void unlock() { m.unlock(); }

private:
	std::mutex m;
};

struct no_lock_policy
{
	no_lock_policy() = default;
	~no_lock_policy() = default;
	no_lock_policy(const no_lock_policy&) = delete;
	no_lock_policy& operator=(const no_lock_policy&) = delete;

	void lock()   { }
	void unlock() { }
};

template <typename LockPolicy>
struct my_lock_guard 
{
	my_lock_guard(LockPolicy& lock_policy) : 
		lock_policy(lock_policy)
	{
		this->lock_policy.lock();
	}

	~my_lock_guard()
	{
		lock_policy.unlock();
	}

private:
	LockPolicy& lock_policy;
};

#include <unordered_map>

template<typename TKey, typename TValue, typename LockPolicy = no_lock_policy>
struct kv_storage
{
	using storage_type = std::unordered_map<TKey, TValue>;

	kv_storage() = default;
	~kv_storage() = default;
	kv_storage(const kv_storage<TKey, TValue>&) = delete;
	kv_storage<TKey, TValue>& operator=(const kv_storage<TKey, TValue>&) = delete;

	void add(const TKey& key, const TValue& value)
	{
		TKey k = key;
		TValue v = value;
		auto kv = std::make_pair<TKey, TValue>(std::move(k), std::move(v));
		
		my_lock_guard<LockPolicy> guard(my_lock);
		storage.insert(kv);
	}

	bool has_key(const TKey& key) const
	{
		storage_type::const_iterator iter, end;
		{
			my_lock_guard<LockPolicy> guard(my_lock);
			iter = storage.find(key);
			end = storage.cend();
		}
		return iter != end;
	}

	bool try_get_value(const TKey& key, TValue& value) const
	{
		storage_type::const_iterator iter, end;
		bool has_key;
		{
			my_lock_guard<LockPolicy> guard(my_lock);
			iter = storage.find(key);
			end = storage.cend();
			has_key = iter != end;
		}
		
		if (has_key)
			value = iter->second;
		return has_key;
	}

private:
	storage_type storage;
	mutable LockPolicy my_lock;
};

#include <string>

#include <iostream>
#include <iomanip>
using std::cout;
using std::boolalpha;

#include <thread>
#include <chrono>

void test_concurrency()
{
	kv_storage<std::string, std::string, mutex_lock_policy> books;
	
	const int ITERS = 10000;

	std::thread t1([&]() {
		for (int i = 0; i < ITERS; ++i)
		{
			std::string ii = std::to_string(i);
			books.add("Euclid's Elements" + ii, "1888009195");
			books.add("Elements of Programming" + ii, "032163537X");
			books.add("From Mathematics to Generic Programming" + ii, "0321942043");
			books.add("Concrete Mathematics: A Foundation for Computer Science" + ii, "0201558025");
			books.add("A Book of Abstract Algebra: Second Edition" + ii, "0486474178");
		}
	});
	
	std::thread t2([&]() {
		for (int i = 0; i < ITERS; ++i)
		{
			std::string ii = std::to_string(i);
			books.add("The Art of Electronics 3rd Edition" + ii, "0521809266");
			books.add("The Art of Mathematics" + ii, "0486450201");
			books.add("Introduction to Algorithms, 3rd Edition" + ii, "0262033844");
			books.add("A History of Mathematics (3rd Edition)" + ii, "0321387007");
			books.add("Introduction to Graph Theory" + ii, "0486678709");
		}
	});

	std::string taocp_1 = "The Art of Computer Programming, Vol. 1";

	std::thread t3([&]() {
		//std::this_thread::sleep_for(std::chrono::milliseconds(250)); //add latency

		cout << boolalpha << books.has_key(taocp_1) << "\n";
		std::string val;
		if (books.try_get_value(taocp_1, val)) cout << val << "\n";
	});
	
	//std::this_thread::sleep_for(std::chrono::milliseconds(250)); //add latency

	books.add(taocp_1, "9780201896831");
	for (int i = 0; i < ITERS; ++i)
	{
		books.add(taocp_1 + std::to_string(i), "9780201896831");
	}
	
	t1.join();
	t2.join();
	t3.join();
}

int policy_sample_main()
{
	kv_storage<std::string, std::string> books;

	std::string taocp_1 = "The Art of Computer Programming, Vol. 1";
	books.add(taocp_1, "9780201896831");
	books.add("The Art of Electronics 3rd Edition", "0521809266");
	books.add("The Art of Mathematics", "0486450201");

	cout << boolalpha << books.has_key(taocp_1) << "\n";
	std::string val;
	if (books.try_get_value(taocp_1, val)) cout << val << "\n";

	test_concurrency();

	return 0;
}

#endif /* _policy_sample_ */