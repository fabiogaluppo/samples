//Sample provided by Fabio Galuppo  
//March 2015  

#include <vector>
#include <utility>
#include <string>
#include <iostream>
#include <algorithm>

template <typename T>
struct InterestRate;

namespace std
{
	template <typename U>
    std::wstring to_wstring(const InterestRate<U>& _Val);

	inline wchar_t to_wstring(wchar_t _Val)
	{
		return _Val;
	}

	inline std::wstring to_wstring(std::wstring _Val)
	{
		return _Val;
	}
};

template<class InputIterator>
static std::wstring mkString(InputIterator first, InputIterator last)
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
static std::wstring mkString(InputIterator first, InputIterator last, const wchar_t* sep)
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
static std::wstring mkString(InputIterator first, InputIterator last, const wchar_t* start, const wchar_t* sep, const wchar_t* end)
{
	return start + mkString(first, last, sep) + end;
}

template<class InputIterator>
static void printLn(InputIterator first, InputIterator last)
{
	std::wcout << mkString(first, last, L"{", L", ", L"}") << L"\n";
}

template<typename T, template <typename, typename> class TContainer>
static void printLn(const TContainer<T, std::allocator<T>>& xs)
{
	printLn(xs.cbegin(), xs.cend());
}

template<typename T>
static void printLn(const T& x)
{
	std::wcout << x << L"\n";
}

static void printLn()
{
	std::wcout << L"\n";
}
