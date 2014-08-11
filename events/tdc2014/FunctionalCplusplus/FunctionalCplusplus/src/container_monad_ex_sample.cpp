//Sample provided by Fabio Galuppo 
//August 2014
//Part of Monadic types prototype for C++

#ifdef _MSC_VER

#include <vector>
#include <string>
#include <iostream>

#include <boost/filesystem.hpp>
using namespace boost::filesystem;

#include "container_monad_ex.hpp"

std::vector<std::string> get_files_as_string(const path& dir)
{
	if (is_regular_file(dir))
		return std::vector<std::string>({ dir.filename().generic_string() });

	std::vector<path> files;
	std::copy_if(directory_iterator(dir), directory_iterator(), std::back_inserter(files), [](const path& p){ return is_regular_file(p); });
	std::vector<std::string> result;
	std::transform(files.cbegin(), files.cend(), std::back_inserter(result), [](const path& p){ return p.filename().generic_string(); });
	return result;
}

void run_container_monad_ex()
{
	std::vector<int> a = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

	auto b = make_monad(a);

	auto c = b.map([](int i){ return 10 * i; });

	path p("..\\..");
	std::vector<path> dir;
	std::copy(directory_iterator(p), directory_iterator(), std::back_inserter(dir));

	auto m = make_monad(dir);
	auto files = m.flat_map<std::string>([](const path& p){ return get_files_as_string(p); });
	files.for_each([](const std::string& file){ std::cout << file << "\n"; });
}

#else

void run_container_monad_ex(){}

#endif