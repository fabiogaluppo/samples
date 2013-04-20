//Sample provided by Fabio Galuppo
#include <vector>
#include <iostream>
#include <amp.h>
#include <amp_math.h>

using namespace concurrency;
using namespace concurrency::fast_math;
//using namespace concurrency::precise_math; //requires double precision

using std::vector;
using std::cout;
using std::ends;
using std::endl;

namespace Computation
{
	struct DataInput
	{
		float a; //angle (in degree)
		float u; //launch velocity
		float t; //time since launch
	};

	struct DataOutput
	{
		float x, y; //position
		float v; //velocity
		float theta; //direction at time (in degree)
	};

	void println(const DataInput& d_in)
	{
		cout << "[Launch angle :" << d_in.a << "]" << ends;
		cout << "[Launch velocity :" << d_in.u << "]" << ends;
		cout << "[time Since Launch :" << d_in.t << "]" << endl;
	}

	void println(const DataOutput& d_out)
	{
		cout << "Horizontal displacement :" << d_out.x << endl;
		cout << "Vertical   displacement :" << d_out.y << endl;
		cout << "Resultant  velocity     :" << d_out.v << endl;
		cout << "Direction  (in degree)  :" << d_out.theta << endl;
	}

	void do_computation()
	{
		vector<DataInput> cpu_in;
		for(unsigned int i = 0; i < 2000; ++i)
		{
			DataInput in = { 45.f, 10.f, i / 1000.f };
			cpu_in.push_back(in);
		}

		array<DataInput> gpu_in(cpu_in.size());
		copy(cpu_in.begin(), cpu_in.end(), gpu_in);
	
		array<DataOutput> gpu_out(cpu_in.size());

		const float PI = 3.141592f;
		const float g = 9.8f;

		parallel_for_each
		(
			gpu_in.extent,
			[&, PI, g](index<1> idx) restrict(amp)
			{
				auto d_in = gpu_in[idx];
				auto angle = d_in.a * PI / 180.f;
				auto ucos = d_in.u * cosf(angle);
				auto usin = d_in.u * sinf(angle);
				auto t = d_in.t;
				auto x = ucos * t;
				auto y = usin * t - g * t * t / 2.f;
				auto vx = ucos;
				auto vy = usin - g * t;
				auto v = sqrtf(vx * vx + vy * vy);
				auto theta = atanf(vy / vx) * 180.f / PI;
			
				DataOutput d_out = { x, y, v, theta };
				gpu_out[idx] = d_out;
			}
		);

		vector<DataOutput> cpu_out = gpu_out;

		for(unsigned int i = 0, l = cpu_in.size(); i < l; ++i)
		{
			println(cpu_in[i]);
			println(cpu_out[i]);
		}
	}

	void run()
	{
		do_computation();
	}
}