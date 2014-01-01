//Sample provided by Fabio Galuppo
//January 2014

#include <type_traits>
#include <algorithm>
#include <functional>
#include <tuple>
#include <vector>
#include <string>
#include <iostream>
#include <typeinfo>
#include <memory>
#include <random>
#include <ctime>

//Lambda Expression BEGIN
template<typename I, typename BinOp> I 
binary_operation(const I& lhs, const I& rhs, BinOp binOp)
{
	static_assert(std::is_arithmetic<I>::value, "I must be a number");
	return binOp(lhs, rhs);
}

template<typename T, size_t N>
std::function<bool(T, T)> 
get_tuple_comparer()
{
	return [](const T& lhs, const T& rhs) { 
		return std::get<N>(lhs) < std::get<N>(rhs); 
	};
}

//Higher-order function and Closure via Lambda Expression
void feature_1()
{
	//function as argument
	int max0 = binary_operation(1, 2, [](int i, int j){ return std::max(i, j); });
	
	std::function<int(int, int)> min = [](int i, int j){ return std::min(i, j); };
	int min0 = binary_operation(1, 2, min);

	//function as return
	using T = std::tuple<short, short>;
	T xs[] = { std::make_tuple(4, 2), std::make_tuple(3, 3), std::make_tuple(2, 4) };
	std::sort(xs, xs + 3, get_tuple_comparer<T, 0>());
	std::sort(xs, xs + 3, get_tuple_comparer<T, 1>());

	//closure
	double x = 10;
	std::function<double(double)> f = [&x](double y){ return x + y; };
	double sum0 = f(20);
}
//Lambda Expression END

//range-for BEGIN
template <typename T> struct ImmutableVector final
{
	typedef std::vector<T> container_type;

	ImmutableVector(std::vector<T> xs) : Xs_(xs)
	{
	}

	typename container_type::const_iterator begin() const
	{
		return Xs_.cbegin();
	}

	typename container_type::const_iterator end() const
	{
		return Xs_.cend();
	}

	ImmutableVector() = delete;
	ImmutableVector(const ImmutableVector&) = delete;
	ImmutableVector& operator=(const ImmutableVector&) = delete;

private:
	std::vector<T> Xs_;
};

void feature_2()
{
	using V = std::vector<double>;
	V xs = { 1.0, 1.0, 2.0, 3.0, 5.0, 8.0, 13.0, 21.0 };

	double total0 = 0.0;
	for (V::const_iterator x = xs.cbegin(); x != xs.cend(); ++x)
		total0 += *x;

	double total1 = 0.0;
	for (double x : xs)
		total1 += x;

	ImmutableVector<int> ys({ 1, 1, 2, 3, 5, 8, 13, 21 });
	double total2 = 0.0;
	for (int y : ys)
		total2 += y;
}
//range-for END

//using alias BEGIN
#if defined(_MSC_VER) 
template <typename Container>
struct random_access_container_wrapper final
{
	using self = random_access_container_wrapper;

	using size_type = typename Container::size_type;
	using const_reference = typename Container::const_reference;
	using reference = typename Container::reference;

	self(std::string description, size_type size) :
		description(description), c(Container(size))
	{
	}

	self(const char* description) :
		self(description, 64)
	{
	}

	self(size_type size) : self("", size)
	{
	}

	template<typename U>
	self(std::initializer_list<U> il) : self(il.size())
	{
		std::copy(il.begin(), il.end(), c.begin());
	}

	const_reference operator[](size_type pos) const
	{
		return *(c.begin() + pos);
	}

	reference operator[](size_type pos)
	{
		return *(c.begin() + pos);
	}

	self(const self&) = delete;

	self& operator=(const self&) = delete;

	self() : self("")
	{
	}

	~random_access_container_wrapper() = default;

private:
	Container c;
	std::string description;
};

template<typename T>
using racw_with_vector = random_access_container_wrapper<std::vector<T>>;

template<typename T> using V = std::vector<T>;

void feature_3()
{
	using POSITIVE_INTEGER = signed int;
	using INT_V = std::vector<int>;
	V<POSITIVE_INTEGER> xs { 1, 2, 3, 4 };
	racw_with_vector<POSITIVE_INTEGER> ys { 1, 2, 3, 4 };

}
#else
void feature_3() {}
#endif
//using alias END

//auto type inference BEGIN
auto feature_4() -> void
{
	auto d = 10.0; //double
	const auto s = "Hello, World!"; //const char*
	auto xs{ 1, 2, 3, 4 }; //std::initializer_list<int>
	auto ys = std::vector<int> { 1, 2, 3, 4 }; //std::vector<int>
	auto zs = std::vector<int*> { &ys[0], &ys[1], &ys[2], &ys[3] }; //std::vector<int*>

	auto it0 = ys.cbegin();
	//instead of
	//std::vector<int>::const_iterator it0 = ys.begin();
	
	for (auto x : xs) //x == int
	{
		std::cout << typeid(x).name() << std::endl;
		break;
	}

	for (const auto& y : ys) //y == const int& 
	{
		std::cout << typeid(y).name() << std::endl;
		break;
	}

	for (auto* z : zs) //z == int*
	{
		std::cout << typeid(z).name() << std::endl;
		break;
	}
}
//auto type inference END

#if defined(_MSC_VER)
//initializer list BEGIN
template <typename Container>
struct container_wrapper final
{
	using self = container_wrapper;

	template<typename U>
	self(std::initializer_list<U> il)
	{
		c.resize(il.size());
		std::copy(il.begin(), il.end(), c.begin());
	}

	operator Container() const
	{
		return c;
	}

	self() = delete;
	self(const self&) = delete;
	self& operator=(const self&) = delete;

	~container_wrapper() = default;

private:
	Container c;
};

void feature_5()
{
	int i{ 1 };

	std::vector<std::string> three_stooges { "Larry", "Curly", "Moe" };

	container_wrapper<std::vector<char>> xs { 'a', 'b', 'c', 'd' };
}
#else
void feature_5() {}
#endif
//initializer list END

//move semantics BEGIN
struct random_functor final
{
	random_functor(int min_inclusive, int max_inclusive, unsigned seed = static_cast<unsigned>(std::time(nullptr))) :
	Engine_(new std::default_random_engine(seed)),
	Rnd_(new std::uniform_int_distribution<int>(min_inclusive, max_inclusive))
	{
	}

	random_functor(int max_exclusive, unsigned seed = static_cast<unsigned>(std::time(nullptr))) :
		Engine_(new std::default_random_engine(seed)),
		Rnd_(new std::uniform_int_distribution<int>(0, max_exclusive - 1))
	{
	}

	auto operator()() const -> int { return (*Rnd_)(*Engine_); }

private:
	std::shared_ptr<std::default_random_engine> Engine_;
	std::shared_ptr<std::uniform_int_distribution<int>> Rnd_;
};

std::vector<int> get_int_vector(size_t size)
{
	std::vector<int> temp;
	random_functor rnd(static_cast<int>(size));
	while (size-- > 0)
		temp.push_back(rnd());
	return temp;
}

struct Class final
{
	Class()
	{
		allocate();
		auto size = SIZE;
		random_functor rnd(static_cast<int>(size));
		while (size-- > 0) Xs_[size] = rnd();
	}

	Class(const Class& that) //copy constructor 
	{
		if (that.Xs_)
		{
			allocate();
			copy(that);
		}
	}

	Class& operator=(const Class& that) //copy assignment
	{
		if (this != &that && that.Xs_) 
			copy(that);
		return *this; 
	}

	Class(Class&& that) //move constructor
	{
		if (that.Xs_)
			move(that);
	}

	Class& operator=(Class&& that) //move assignment
	{
		if (this != &that && that.Xs_)
			move(that);
		return *this;
	}

	~Class() { deallocate(); }

private:
	void allocate() { Xs_ = new int[SIZE]; }

	void deallocate() 
	{ 
		if (Xs_)
		{
			delete[] Xs_;
			Xs_ = nullptr;
		}
	}

	void copy(const Class& that)
	{
		auto size = SIZE;
		while (size-- > 0) Xs_[size] = that.Xs_[size];
	}

	void move(Class& that)
	{
		Xs_ = that.Xs_;
		that.Xs_ = nullptr;
	}

	static const size_t SIZE = 10;

	int* Xs_ = nullptr;
};

Class make_Class() { return Class(); }

void feature_6()
{
	auto xs = get_int_vector(5); //std::vector move constructor

	Class c0; //default constructor
	Class c1 { c0 }; //copy constructor
	
	Class c2 { std::move(make_Class()) }; //move constructor
	Class c3;
	c3 = std::move(c0); //move assignment

	/* Sequence of: 
		move constructor
		move constructor
		move constructor
		move constructor
		move constructor
		move constructor
		move constructor
		move assignment
		move assignment
		move constructor
		move assignment
		move assignment */
	std::vector<Class> ys;
	ys.push_back(make_Class()); 
	ys.push_back(make_Class()); 
	ys.insert(ys.begin(), make_Class());
};
//move semantics END

//variadic templates BEGIN
template <typename T, typename... Ts> struct List
{
	T head;
	List<Ts...> tail;
};

template<typename T> struct List<T>
{
	T head;
};

void println_all() { std::cout << std::endl; }

template<typename Arg, typename... Args>
void println_all(Arg a, Args... args)
{
	unsigned int size = sizeof...(args);
	if (size >= 0)
	{
		std::cout << a << " ";
		println_all(args...);
	}
}

template<typename... Items>
void println_all_uniform(Items... items)
{
	//Items must be uniform (same type for all args)
	auto xs = { items... };
	for (auto& x : xs)
		std::cout << x << " ";
	std::cout << std::endl;
}

void feature_7()
{
	List<int, int, int> xs;
	xs.head = 1;
	xs.tail.head = 2;
	xs.tail.tail.head = 3;
	//xs.tail.tail.tail ... invalid

	List<int, int, int, int> ys{ 1 };
	List<int, int, int> ys1 = ys.tail;
	List<int, int> ys2 = ys1.tail;
	List<int> ys3 = ys2.tail;

	println_all(1, 2.0, "3", '4');

	println_all_uniform(1.f, 2.f, 3.f, 4.f);

	auto t0 = std::make_tuple(1, 2.0, "3", '4');
	std::cout << std::get<0>(t0) << " "
			<< std::get<1>(t0) << " "
			<< std::get<2>(t0) << " "
			<< std::get<3>(t0) << std::endl;
	
	auto t1 = std::make_tuple(1.f, 2.f, 3.f, 4.f);
	std::cout << std::get<0>(t1) << " "
		<< std::get<1>(t1) << " "
		<< std::get<2>(t1) << " "
		<< std::get<3>(t1) << std::endl;
}
//variadic templates END

int main()
{
	feature_1();
	feature_2();
	feature_3();
	feature_4();
	feature_5();
	feature_6();
	feature_7();
}