//Sample provided by Fabio Galuppo  
//June 2017

#ifndef REGULAR_TYPE_HPP
#define REGULAR_TYPE_HPP

#define PositiveInteger typename

#include <type_traits>
#include <cassert>
#include <string>
#include <utility>
#include <algorithm>
#include <sstream>

#include <iostream>

template<typename T>
inline void stable_sort_2(T& a, T& b)
{
	if (b < a) std::swap(a, b);

	//post-condition:
	assert(a == std::min(a, b)); //equality and min
}

template <PositiveInteger T>
struct straight_line_segment_1d final
{
    static_assert(std::is_arithmetic<T>::value, "not an arithmetic type");

	//default constructor
	straight_line_segment_1d() :
		straight_line_segment_1d(T(0), T(1))
	{
	}
	
	//constructor
	straight_line_segment_1d(T a, T b)
		: a(a), b(b)
	{
		assert(a >= T(0));
		assert(b >= T(0));
		//convention a < b
		stable_sort_2(a, b);
		assert(length(a, b) > 0);
	}

	//copy constructor
	straight_line_segment_1d(const straight_line_segment_1d<T>& that)
		: a(that.a), b(that.b), label(that.label)
	{
	}

	//copy assignment
	straight_line_segment_1d& operator=(const straight_line_segment_1d<T>& that)
	{
		if (this != &that)
		{
			a = that.a;
			b = that.b;
            label = that.label;
		}
		return *this;
	}

	//default destructor
	~straight_line_segment_1d() = default;

	//equality
	bool operator==(const straight_line_segment_1d<T>& that) const
	{
		if (this == &that)
			return true;
		return length() == that.length();
	}

	//inequality
	bool operator!=(const straight_line_segment_1d<T>& that) const 
	{
		return !operator==(that);
	}

	//strict totally ordered
	bool operator<(const straight_line_segment_1d<T>& that) const
	{
        if (length() == that.length()) 
        {
            if (A() == that.A())
                return B() < that.B();
            return A() < that.A();
        } 
        return length() < that.length();
	}

	bool operator>(const straight_line_segment_1d<T>& that) const
	{
		return that < *this;
	}

	bool operator<=(const straight_line_segment_1d<T>& that) const
	{
		return !(that < *this);
	}

	bool operator>=(const straight_line_segment_1d<T>& that) const
	{
		return !(*this < that);
	}

    size_t length() const { return length(a, b); }

	T A() const { return a; }
	T B() const { return b; }

    const char* get_label() const { return label.c_str(); }

    void set_label(const char* value = "") { label = value; }
	
    std::string to_string() const
	{
        std::stringstream ss;
        if (!label.empty())
            ss << label << " = ";
        ss << a << " -- " << std::to_string(length()) 
           << " -- " << std::to_string(b);
        return ss.str();
    }

private:
	static inline size_t length(T a, T b) 
	{
		assert(a >= T(0));
		assert(b >= T(0));
		return b - a;
	}

private:
	T a, b; /* endpoints */
    std::string label;
};

#endif /* REGULAR_TYPE_HPP */