//Sample provided by Fabio Galuppo  
//March 2016 

#ifndef _regular_type_sample_
#define _regular_type_sample_

#define PositiveInteger typename

#include "ordering_sample.hpp"
#include <cassert>
#include <string>

template <PositiveInteger T>
struct straight_line_segment_1d final
{
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
		: a(that.a), b(that.b)
	{
	}

	//copy assignment
	straight_line_segment_1d& operator=(const straight_line_segment_1d<T>& that)
	{
		if (this != &that)
		{
			a = that.a;
			b = that.b;
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

	std::string to_string() const
	{
		return std::to_string(a) + " -- " + std::to_string(length()) + 
			" -- " + std::to_string(b);
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
};

#include <iostream>
#include <iomanip>
using std::cout;
using std::boolalpha;

template <PositiveInteger T>
static inline std::ostream& operator<< (std::ostream& os, straight_line_segment_1d<T> val)
{
	os << val.to_string();
	return os;
}

#include <vector>
#include "ordering_sample.hpp"
#include <algorithm>
#include <iterator>

int regular_type_sample_main()
{
	using straight_line = straight_line_segment_1d<short>; //type alias with template instantiation 

	straight_line ab(2, 4); //constructor
	straight_line cd; //default constructor
	straight_line ef; //default constructor and
	ef = straight_line(1, 6); //copy assignment
	straight_line gh(straight_line(0, 2)); //copy constructor
	
	cout << ab << " == " << gh << "? " << boolalpha << (ab == gh) << "\n";
	cout << cd << " == " << ef << "? " << boolalpha << (cd == ef) << "\n";
	cout << cd << " != " << ef << "? " << boolalpha << (cd != ef) << "\n";
	cout << ab << " <  " << gh << "? " << boolalpha << (ab < gh)  << "\n";
	cout << ab << " <= " << gh << "? " << boolalpha << (ab <= gh) << "\n";
	cout << gh << " >= " << ab << "? " << boolalpha << (gh >= ab) << "\n";
	cout << gh << " >  " << ab << "? " << boolalpha << (gh > ab)  << "\n";

	using straight_lines = std::vector<straight_line>; //type alias with template instantiation 
	straight_lines xs { ab, cd, ef, gh };
	
	cout << "median of " << xs[0] << ", " << xs[1] << " and " << xs[2] << " is ";
	cout << immutable_median_of_3(xs[0], xs[1], xs[2]) << "\n";

	std::sort(xs.begin(), xs.end());

	cout << "median of " << xs[0] << ", " << xs[1] << " and " << xs[2] << " is ";
	auto m = immutable_median_of_3(xs[0], xs[1], xs[2]);
	cout << m << "\n";

	straight_lines::iterator bound = std::partition(xs.begin(), xs.end(), 
		[&m](straight_line& line) { return line <= m; });
	cout << std::distance(bound, xs.end()) << " line(s) greater than " << m << ":\n";	
	std::copy(bound, xs.end(), std::ostream_iterator<straight_line>(cout, "\n"));
	cout << "other side of the boundary:\n";
	std::copy(xs.begin(), bound, std::ostream_iterator<straight_line>(cout, "\n"));
	
	return 0;
}

#endif /* _regular_type_sample_ */
