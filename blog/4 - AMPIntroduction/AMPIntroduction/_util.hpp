//Sample provided by Fabio Galuppo
#pragma once

#include <vector>
#include <iostream>

unsigned int clamp(unsigned v, unsigned min, unsigned max)
{
	if(v > max) return max;
	if(v < min) return min;
	return v;
}

void print_matrix(const std::vector<float>& a, int rows, int cols, std::streamsize new_precision = 0, std::streamsize new_width = 0)
{
	using std::cout;
	using std::ios;
	
	for(int i = 0, l = clamp(rows, 1, 10); i < l; ++i)
	{
		for(int j = 0, m = clamp(cols, 1, 10); j < m; ++j)
		{
			cout.precision(new_precision > 0 ? new_precision : cout.precision());
			cout.setf(ios::fixed, ios::floatfield);
			cout.width(new_width > 0 ? new_width : cout.width());

			cout << a[i * cols + j] << " ";

			cout.unsetf(ios::floatfield);
		}

		cout << "\n";
	}
}