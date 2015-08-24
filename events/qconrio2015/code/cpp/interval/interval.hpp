//Sample provided by Fabio Galuppo 
//August 2015

#ifndef _interval_
#define _interval_

#include <utility>
#include <chrono>
#include <tuple>
#include <numeric>

struct time_line_segment_1D final
{
	typedef std::chrono::high_resolution_clock::time_point value_type;
	typedef std::chrono::duration<double> size_type;
	
	time_line_segment_1D(value_type left, value_type right)
	{
		if (left > right)
			std::swap(left, right);
		Left_ = left;
		Right_ = right;
	}

	const value_type left() const { return Left_; }

	const value_type right() const { return Right_; }

	const size_type size() const { return right() - left(); }

	/*const size_type size_squared() const 
	{ 
		auto len = right() - left();
		return std::chrono::duration<double>(len.count() * len.count());
	}*/

	const bool intersect(const time_line_segment_1D& that) const
	{
		if (that.right() < left() || right() < that.left())
			return false;
		return true;
	}

	const bool belongs_to(value_type value) const
	{
		return left() <= value && right() <= value;
	}

private:
	value_type Left_, Right_;
};

struct time_ruler
{
	time_ruler() : Start_(now()) {}

	const time_line_segment_1D get() const
	{
		return time_line_segment_1D(Start_, now());
	}

private:
	static std::chrono::high_resolution_clock::time_point now()
	{
		return std::chrono::high_resolution_clock::now();
	}

	std::chrono::high_resolution_clock::time_point Start_;
};

#include <vector>
#include <algorithm>
#include <cmath>
#include <limits>

struct compute_info final
{
	//TODO: private (friend...)
	compute_info(size_t count, std::vector<time_line_segment_1D> disjointed_lines)
		: Count_(count), DisjointedLines_(disjointed_lines)
	{
	}

	const time_line_segment_1D::size_type average() const 
	{
		double result = std::numeric_limits<double>::infinity();
		if (count() > 0)
			result = total().count() / static_cast<double>(count());
		return time_line_segment_1D::size_type(result);
	}

	const size_t count() const 
	{ 
		return Count_; 
	}
	
	const time_line_segment_1D::size_type total() const 
	{ 
		time_line_segment_1D::size_type acc;
		return std::accumulate(DisjointedLines_.cbegin(), DisjointedLines_.cend(), acc, 
					[](time_line_segment_1D::size_type& acc, const time_line_segment_1D& x){
						return acc + x.size();
					});
	}

	const time_line_segment_1D::size_type stddev() const
	{
		auto avg = average();
		time_line_segment_1D::size_type acc;
		acc = std::accumulate(DisjointedLines_.cbegin(), DisjointedLines_.cend(), acc,
			[avg](time_line_segment_1D::size_type& acc, const time_line_segment_1D& x){
				auto y = x.size() - avg;
				return acc + (time_line_segment_1D::size_type(y.count() * y.count()));
		});
		return time_line_segment_1D::size_type(acc.count() / count());
	}

	const std::tuple<time_line_segment_1D, time_line_segment_1D> minmax() const
	{
		auto minmax = std::minmax_element(DisjointedLines_.cbegin(), DisjointedLines_.cend(), 
							[](const time_line_segment_1D& x, const time_line_segment_1D& y){
								return x.size() < y.size();
							});
		return std::make_tuple(*(minmax.first), *(minmax.second));
	}

private:
	size_t Count_;	
	std::vector<time_line_segment_1D> DisjointedLines_;
};

#include <mutex>

//#include <iostream> //DEBUG

struct time_line_segment_1D_collection final
{
	void collect(time_line_segment_1D value)
	{
		std::lock_guard<std::mutex> lock(Lock_);

		Timeline_.push_back(value);
	}

	compute_info compute()
	{
		std::vector<time_line_segment_1D> disjointed_lines;
		//time_line_segment_1D::size_type total;
		size_t count = 0;
		{
			std::lock_guard<std::mutex> lock(Lock_);

			count = Timeline_.size();
			
			std::sort(Timeline_.begin(), Timeline_.end(),
				[](const time_line_segment_1D& lhs, const time_line_segment_1D& rhs) -> bool {
				//left ascending order
				return (lhs.left() > rhs.left() || lhs.right() > rhs.right());
			});

			if (Timeline_.size() > 0)
			{
				std::vector<time_line_segment_1D>::const_iterator it = Timeline_.cbegin();
				auto x = *it;
				while (++it != Timeline_.cend())
				{
					if (x.intersect(*it))
					{
						x = time_line_segment_1D(std::min(x.left(), it->left()),
							std::max(x.right(), it->right()));						
					}
					else
					{
						disjointed_lines.push_back(x);
						//total += x.size();
						x = *it;
					}
				}
				disjointed_lines.push_back(x);
				//total += x.size();

				////DEBUG
				//long long result = std::chrono::duration_cast<std::chrono::seconds>(total).count();
				//std::cout << result;
				////DEBUG
			}
		}
        
        /*
        std::cout << "All segments:\n"; //DEBUG
        for (auto& ls : Timeline_)
            std::cout << std::chrono::duration_cast<std::chrono::milliseconds>(ls.size()).count() << "\n";
            
        std::cout << "Disjointed segments:\n"; //DEBUG
        for (auto& ls : disjointed_lines)
            std::cout << std::chrono::duration_cast<std::chrono::milliseconds>(ls.size()).count() << "\n";
        */
        
		return compute_info(count, disjointed_lines);
	}

private:
	std::vector<time_line_segment_1D> Timeline_;
	std::mutex Lock_;
};

#endif /* _interval_ */