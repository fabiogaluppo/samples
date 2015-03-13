//Sample provided by Fabio Galuppo  
//March 2015  

#include "Util.hpp"

#include <vector>
#include <array>
#include <unordered_map>
#include <unordered_set>
#include <tuple>
#include <algorithm>
#include <iostream>
#include <numeric>
#include <limits>
#include <functional>
#include <cmath>

static void Sets()
{
	std::vector<int> xs{ 4, 2, 1, 5, 7, 2, 1, 2 };
	std::vector<int> ys{ 8, 1, 2, 4, 9, 1, 2, 4 };

	printLn(xs);
	printLn(ys);

	std::sort(xs.begin(), xs.end());
	std::sort(ys.begin(), ys.end());

	std::vector<int>::iterator it;

	std::vector<int> xs_intersect_ys(8);
	it = std::set_intersection(xs.begin(), xs.end(), ys.begin(), ys.end(), xs_intersect_ys.begin()); //requires order
	xs_intersect_ys.resize(it - xs_intersect_ys.cbegin());

	it = std::unique(xs_intersect_ys.begin(), xs_intersect_ys.end()); //keep only distinct elements
	xs_intersect_ys.resize(it - xs_intersect_ys.cbegin());

	printLn(xs_intersect_ys);

	std::vector<int> ys_intersect_xs(8);
	it = std::set_intersection(ys.begin(), ys.end(), xs.begin(), xs.end(), ys_intersect_xs.begin());
	ys_intersect_xs.resize(it - ys_intersect_xs.cbegin());

	it = std::unique(ys_intersect_xs.begin(), ys_intersect_xs.end());
	ys_intersect_xs.resize(it - ys_intersect_xs.cbegin());

	printLn(ys_intersect_xs);

	bool equals = std::equal(xs_intersect_ys.cbegin(), xs_intersect_ys.cend(), ys_intersect_xs.cbegin(), [](int lhs, int rhs){ return lhs == rhs; });

	std::wcout << as_wstring(equals) << L"\n";
}

static void CartesianProduct()
{
	std::array<wchar_t, 3> chars = { L'a', L'b', L'c' };
	std::array<int, 3> nums = { 1, 2, 3 };

	std::vector<std::tuple<wchar_t, int>> chars_cross_nums;
	for (auto c : chars)
		for (auto n : nums)
			chars_cross_nums.push_back(std::make_tuple(c, n));

	std::vector<std::tuple<int, wchar_t>> nums_cross_chars;
	for (auto n : nums)
		for (auto c : chars)
			nums_cross_chars.push_back(std::make_tuple(n, c));

	printLn(chars_cross_nums);
	printLn(nums_cross_chars);
}

static void Relation()
{
	std::array<std::wstring, 4> Domain = { L"Mark Twain", L"Lewis Carroll", L"Charles Dickens", L"Stephen King" };
	std::array<std::wstring, 4> CoDomain = { L"A Christmas Carol", L"Alice's Adventures in Wonderland", L"The Adventures of Tom Sawyer", L"The Left Hand of Darkness" };
	printLn(Domain);
	printLn(CoDomain);

	std::unordered_map<std::wstring, std::wstring> R;
	R.emplace(Domain[0], CoDomain[2]);
	R.emplace(Domain[1], CoDomain[1]);
	R.emplace(Domain[2], CoDomain[0]);
	printLn(R);

	std::vector<std::wstring> values, keys;
	for (const auto& r : R)
	{
		values.push_back(std::get<1>(r));
		keys.push_back(std::get<0>(r));
	}
	std::sort(values.begin(), values.end());
	std::sort(Domain.begin(), Domain.end());
	std::sort(keys.begin(), keys.end());
	std::sort(CoDomain.begin(), CoDomain.end());

	std::vector<std::wstring>::iterator it;

	std::vector<std::wstring> Image(CoDomain.size());
	it = std::set_intersection(CoDomain.cbegin(), CoDomain.cend(), values.cbegin(), values.cend(), Image.begin());
	Image.resize(it - Image.cbegin());
	printLn(Image);

	std::vector<std::wstring> PreImage(Domain.size());
	it = std::set_intersection(Domain.cbegin(), Domain.cend(), keys.cbegin(), keys.cend(), PreImage.begin());
	PreImage.resize(it - PreImage.cbegin());
	printLn(PreImage);

	std::unordered_map<std::wstring, std::wstring> R_inverse;
	for (const auto& r : R)
		R_inverse.insert(transpose(r)); //R_inverse.emplace(r.second, r.first); //transpose alternative
	printLn(R_inverse);
}

typedef unsigned char byte;

static byte /* CoDomain */ duplicate(byte /* Domain */ x)
{
	byte y = 2 * x /* pre-image */;
	return y /* image */;
}

template<class InputIterator>
static byte duplicate(byte x, InputIterator Domain_first, InputIterator Domain_last)
{
	byte y = (2 * x) % std::distance(Domain_first, Domain_last); //modular arithmetic
	return y;
}

static void Functions()
{
	std::wcout << duplicate(128) << L"\n";

	std::vector<byte> byte_Domain;
	byte_Domain.resize(1 + std::numeric_limits<byte>::max() - std::numeric_limits<byte>::min());
	std::iota(byte_Domain.begin(), byte_Domain.end(), std::numeric_limits<byte>::min());
	printLn(byte_Domain);

	std::unordered_set<byte> Image;
	for (auto x : byte_Domain)
		Image.insert(duplicate(x));
	printLn(Image);

	std::vector<byte> byte_PreImage(byte_Domain.cbegin(), byte_Domain.cbegin() + 5);
	std::unordered_set<byte> Image2;
	for (auto x : byte_Domain)
		Image2.insert(duplicate(x, byte_PreImage.cbegin(), byte_PreImage.cend()));
	printLn(byte_PreImage);
	printLn(Image2);
}

template<class T, class U, class V>
inline std::function<V(T)> compose(std::function<V(U)> g, std::function<U(T)> f)
{
	return [=](T x) -> V { return g(f(x)); };
}

static void Composition()
{
	std::function<int(int)> f = [](int x) { return 2 * x; };
	std::function<int(int)> g = [](int x) { return x + 1; };

	{
		//h = g . f
		auto h = compose(g, f);
		std::wcout << h(1) << L"\n";
		std::wcout << h(2) << L"\n";
	}

	{
		//h = f . g
		auto h = compose(f, g);
		std::wcout << h(1) << L"\n";
		std::wcout << h(2) << L"\n";
	}

	{
		//f . (f . g)
		auto h = compose(f, compose(f, g));
		//(f . f) . g
		auto i = compose(compose(f, f), g);

		bool isAssociative = h(10) == i(10);
		std::wcout << L"Is function composition an associative binary operation? " << as_wstring(isAssociative) << L"\n";
	}
}

struct Msg
{
	Msg(std::wstring a, int b, double c)
		: A_(a), B_(b), C_(c) {}

	const std::wstring& get_A() const { return A_; }
	int get_B() const { return B_; }
	double get_C() const { return C_; }

	void set_A(std::wstring& value) { A_ = value; }
	void set_B(int value) { B_ = value; }
	void set_C(double value) { C_ = value; }

	std::wstring to_wstring()
	{
		std::wstring temp;
		temp += L"{A: \"";
		temp += A_;
		temp += L"\", B: ";
		temp += std::to_wstring(B_);
		temp += L", C: ";
		temp += std::to_wstring(C_);
		temp += L"}";
		return temp;
	}

	Msg() = default;
	Msg(const Msg&) = default;
	Msg& operator=(const Msg&) = default;
	~Msg() = default;

private:
	friend class boost::serialization::access;

	template <typename Archive>
	void serialize(Archive &ar, const unsigned int version)
	{
		ar & A_; ar & B_; ar & C_;
	}

private:
	std::wstring A_;
	int B_;
	double C_;
};

static void InverseFunctions()
{
	std::function<double(double)> lg = [](double x) { return std::log2(x); };
	std::function<double(double)> powerOf2 = [](double x) { return std::pow(2., x); };
	std::wcout << lg(8.0) << L"\n";
	std::wcout << powerOf2(3.0) << L"\n";
	std::wcout << compose(powerOf2, lg)(8.0) << L"\n";
	std::wcout << compose(lg, powerOf2)(3.0) << L"\n";

	Msg msg0{ L"Hello World", 123, 456.789 };
	std::wcout << msg0.to_wstring() << L"\n";

	auto ss = serializeToStream(msg0);
	std::cout << ss.str() << "\n";

	Msg msg1 = deserializeFromStream<Msg>(ss);
	std::wcout << msg1.to_wstring() << L"\n";

	std::function<Msg(std::stringstream)> g = [](std::stringstream& ss) { return deserializeFromStream<Msg>(ss); };
	std::function<std::stringstream(Msg)> f = [](const Msg& msg) { return serializeToStream(msg); };
	auto msg2 = compose(g, f)(msg0);
	std::wcout << msg2.to_wstring() << L"\n";
}

int main()
{
	run(Sets, L"Sets");
	run(CartesianProduct, L"CartesianProduct");
	run(Relation, L"Relation");
	run(Functions, L"Functions");
	run(Composition, L"Composition");
	run(InverseFunctions, L"InverseFunctions");

	std::cin.get();
	return 0;
}
