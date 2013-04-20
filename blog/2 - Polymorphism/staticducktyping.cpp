//g++ -std=c++0x -Wall staticducktyping.cpp

#include <iostream>
#include <cctype>
#include <algorithm>
#include <functional>
#include <string>

using namespace std;

struct X
{
  string M(const string& value)
  {  
    string result(value);
    transform( value.begin(), value.end(), result.begin(), ptr_fun<int, int>( toupper ) );
    return result;
  }
};

struct Y
{
  string M(const string& value)
  {
    string result(value);
    reverse( result.begin(), result.end() );
    return result;
  }
};

template <typename T> string selector (T&& id, const string& value) { return id.M(value); }

template <typename T> void m (T&& id, const char* value)
{
  cout << value << " -> " << selector(id, value) << endl;
}

int main(int argc, char* argv[])
{
  m(X(), "Hello");
  m(Y(), "World");

  return 0;
}