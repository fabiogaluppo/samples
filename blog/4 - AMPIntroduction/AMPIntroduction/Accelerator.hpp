//Sample provided by Fabio Galuppo
#include <iostream>
#include <algorithm>

#include "_generator.hpp" 
#include "_util.hpp"

#include <amp.h>

using namespace concurrency;

namespace Accelerator
{
	void display_accelerator_properties(const accelerator& a)
	{
		using std::wcout;

		wcout << L"description: " << a.description << L"\n";
		wcout << L"dedicated_memory: " << a.dedicated_memory << L"\n";
		wcout << L"device_path: " << a.device_path << L"\n";
		wcout << L"is_debug: " << a.is_debug << L"\n";
		wcout << L"is_emulated: " << a.is_emulated << L"\n";
		wcout << L"has_display: " << a.has_display << L"\n";                
		wcout << L"version: " << HIWORD(a.version) << '.' << LOWORD(a.version) << L"\n";
		wcout << L"\n";
	}
	
	void transform(const accelerator_view& av)
	{
		using std::generate;

		const int N = 10;
		
		vector<float> vec(N); 
		
		PositiveIntegerGenerator gen;
		generate(vec.begin(), vec.end(), [&gen](){ return gen.next(); });
		
		print_matrix(vec, 1, N, 2, 6);

		extent<1> e(N);
		array_view<float, 1> vec_view(e, vec);
		
		parallel_for_each
		( 
			av,
			vec_view.extent, 
			[vec_view](index<1> idx) restrict (amp) 
			{ 
				vec_view[idx] *= 2.f; 
			}
		);
		vec_view.synchronize();
		
		print_matrix(vec, 1, N, 2, 6);
	}

	void using_and_querying_accelerator()
	{
		using std::wcout;

		wcout << "----- hardware accelerator -----\n";

		accelerator hardware_accelerator(accelerator::default_accelerator);
		accelerator_view hardware_view = hardware_accelerator.default_view;

		display_accelerator_properties(hardware_accelerator);
	
		transform(hardware_view);

		wcout << "----- emulator accelerator -----\n";

		accelerator emulated_accelerator(accelerator::direct3d_ref);
		accelerator_view emulated_view = emulated_accelerator.default_view;

		display_accelerator_properties(emulated_accelerator);

		transform(emulated_view);

		wcout << "----- enum all accelerator -----\n"; 
		
		auto all_accelerators = accelerator::get_all();
		for(auto a = all_accelerators.cbegin(); a != all_accelerators.cend(); ++a)
		{
			display_accelerator_properties(*a);
		}
	}

	void run()
	{
		using_and_querying_accelerator();
	}
}