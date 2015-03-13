//Sample provided by Fabio Galuppo  
//March 2015  

#include <vector>
#include <unordered_map>
#include <tuple>
#include <utility>
#include <string>
#include <iostream>
#include <algorithm>

namespace std
{
	inline wchar_t to_wstring(wchar_t _Val)
	{
		return _Val;
	}

	inline std::wstring to_wstring(const std::wstring& _Val)
	{
		return _Val;
	}

	template<typename T1, typename T2>
	inline std::wstring to_wstring(const std::tuple<T1, T2>& _Val)
	{
		// convert std::tuple<T1, T2> to wstring
		std::wstring temp;
		temp += L"(";
		temp += std::to_wstring(std::get<0>(_Val));
		temp += L", ";
		temp += std::to_wstring(std::get<1>(_Val));
		temp += L")";
		return temp;
	}
};

template<class InputIterator>
static std::wstring make_string(InputIterator first, InputIterator last)
{
	std::wstring temp;
	while (first != last)
	{
		temp += std::to_wstring(*first);
		++first;
	}
	return temp;
}

template<class InputIterator>
static std::wstring make_wstring(InputIterator first, InputIterator last, const wchar_t* sep)
{
	std::wstring temp;
	if (first != last)
	{
		temp += std::to_wstring(*first);
		++first;
	}
	while (first != last)
	{
		temp += sep;
		temp += std::to_wstring(*first);
		++first;
	}
	return temp;
}

template<class InputIterator>
static std::wstring make_wstring(InputIterator first, InputIterator last, const wchar_t* start, const wchar_t* sep, const wchar_t* end)
{
	return start + make_wstring(first, last, sep) + end;
}

template<typename T1, typename T2>
inline std::pair<T2, T1> transpose(const std::pair<T1, T2>& x)
{
	return std::make_pair(x.second, x.first);
}

template<class InputIterator>
static void printLn(InputIterator first, InputIterator last)
{
	std::wcout << make_wstring(first, last, L"{", L", ", L"}") << L"\n";
}

template<class TContainer>
static void printLn(const TContainer& xs)
{
	printLn(xs.cbegin(), xs.cend());
}

template<typename T1, typename T2>
static void printLn(const std::unordered_map<T1, T2>& xs)
{
	auto size = std::distance(xs.cbegin(), xs.cend());
	std::vector<std::wstring> temp;
	if (size > 0)
	{
		temp.resize(size);
		std::transform(xs.cbegin(), xs.cend(), temp.begin(), [](const std::pair<T1, T2>& x) {
			std::wstring s;
			s += std::to_wstring(std::get<0>(x));
			s += L" -> ";
			s += std::to_wstring(std::get<1>(x));
			return s;
		});
	}
	printLn(temp);
}

template<class Function>
static void run(Function f, const wchar_t* title)
{
	std::wcout << title << L":\n";
	f();
	std::wcout << std::wstring(15, L'-') << L"\n";
}

static const wchar_t* as_wstring(bool x)
{
	return x ? L"true" : L"false";
}

#include <boost/archive/text_oarchive.hpp>
#include <boost/archive/text_iarchive.hpp>
#include <sstream>

template <typename T>
std::stringstream serializeToStream(const T& x)
{
	std::stringstream ss;
	boost::archive::text_oarchive oa{ ss };
	oa << x;
	return ss;
}

template <typename T>
T deserializeFromStream(std::stringstream& ss)
{
	boost::archive::text_iarchive ia{ ss };
	T x;
	ia >> x;
	return x;
}
