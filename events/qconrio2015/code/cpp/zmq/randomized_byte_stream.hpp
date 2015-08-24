//Sample provided by Fabio Galuppo 
//August 2015

#ifndef _randomized_byte_stream_
#define _randomized_byte_stream_

#include <algorithm>
#include <functional>
#include <random>
#include <vector>
#include <string>

struct randomized_byte_stream final
{
    typedef std::vector<unsigned char> value_type;
    typedef size_t size_type;
    
    //explicit constructor
    explicit randomized_byte_stream(size_type size) : 
        Rnd_(std::bind(std::uniform_int_distribution<short>(0, 255), 
             std::default_random_engine()))
    {
        Data_.resize(size);
    }
    
    const size_type size() const { return Data_.size(); }
    
    const value_type::const_pointer next()
	{
        for (auto& iter : Data_) iter = static_cast<unsigned char>(Rnd_());		
		return data();
	}
    
    const value_type::const_pointer zero()
	{
        for (auto& iter : Data_) iter = 0;		
		return data();
	}
    
    const bool zeroed() const 
    { 
        return std::all_of(Data_.cbegin(), Data_.cend(), [](unsigned char x){ return x == 0; }); 
    }
    
    const value_type::const_pointer data() const { return Data_.data(); }
    
    void* raw_pointer() const { return reinterpret_cast<void*>(const_cast<value_type::pointer>(data())); }
    
    randomized_byte_stream(const randomized_byte_stream&) = delete; //copy constructor
	randomized_byte_stream& operator=(const randomized_byte_stream&) = delete; //copy assignment
	randomized_byte_stream() = delete; //constructor
    ~randomized_byte_stream() = default; //destructor
    
    friend std::string to_string(const value_type& _Val);
	friend std::string to_string(const randomized_byte_stream& _Val);
    friend struct _queue;
    
private:
    value_type Data_;
    std::function<short(void)> Rnd_;
};

#include <sstream>
#include <iomanip>

std::string to_string(const randomized_byte_stream::value_type& _Val)
{
	std::stringstream ss;
	for (const auto& iter : _Val)
		ss << std::setw(2) << std::hex << std::uppercase 
           << std::setfill('0') << static_cast<short>(iter);
	return ss.str();
}

std::string to_string(const randomized_byte_stream& _Val)
{
	return to_string(_Val.Data_);
}

#include <queue>

struct _queue final
{
	typedef std::queue<randomized_byte_stream::value_type> value_type;

	void enqueue(const randomized_byte_stream& b) { Q_.push(b.Data_); }

	value_type::value_type dequeue()
	{
		auto x = std::move(Q_.front());
		Q_.pop();
		return std::move(x);
	}

	const value_type::size_type size() const { return Q_.size(); }
    
    _queue(const _queue&) = delete; //copy constructor
	_queue& operator=(const _queue&) = delete; //copy assignment
	_queue() = default; //constructor
    ~_queue() = default; //destructor

private:
	value_type Q_;
};

#endif /* _randomized_byte_stream_ */