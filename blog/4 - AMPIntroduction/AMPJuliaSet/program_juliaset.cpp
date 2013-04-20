//Sample provided by Fabio Galuppo
//Inspired by JuliaSet Sample in CUDA by Example (ISBN-10: 0131387685)

#include <windows.h>
#include <amp.h>
#include <vector>
#include <ctime>
#include <sstream>

#pragma comment (lib, "opengl32.lib")
#pragma comment (lib, ".\\glut-3.7.6-bin\\glut32.lib")

#include ".\glut-3.7.6-bin\glut.h"
#include ".\glut-3.7.6-bin\glext.h"

using namespace concurrency;

typedef unsigned int uint32;

struct ComplexNumber 
{
    float r, i;
    
	ComplexNumber( float a, float b ) restrict(cpu, amp) : r(a), i(b) {}
    
	float magnitude2( void ) restrict(cpu, amp) 
	{
        return r * r + i * i;
    }
    
	ComplexNumber operator*(const ComplexNumber& a) restrict(cpu, amp) 
	{
        return ComplexNumber(r * a.r - i * a.i, i * a.r + r * a.i);
    }

    ComplexNumber operator+(const ComplexNumber& a) restrict(cpu, amp)
	{
        return ComplexNumber(r + a.r, i + a.i);
    }
};

namespace Julia
{
	const int DIM = 640; //1024;

	std::vector<uint32> Buffer(DIM * DIM);

	class App
	{
		static void Draw()
		{
			glClearColor( 0.f, 0.f, 0.f, 1.f );
			glShadeModel( GL_FLAT );
			glClear( GL_COLOR_BUFFER_BIT );
			glDrawPixels( DIM, DIM, GL_RGBA, GL_UNSIGNED_INT_8_8_8_8, &Buffer[0] );
			glFlush();
		}

		static void Key(unsigned char key, int x, int y) 
		{
			switch (key) 
			{
				case 27:
					exit(0);
			}
		}

	public:
		static void Run(const char* msg, double d) 
		{
			int c = 1;
			char* dummy = "";
			glutInit( &c, &dummy );
			glutInitDisplayMode( GLUT_SINGLE | GLUT_RGBA );
			glutInitWindowSize( DIM, DIM );

			std::stringstream ss;
			ss.precision(4);
			ss << "Julia Set - Fabio Galuppo - ";
			ss << msg << " - ";
			ss << d << " s"; 

			glutCreateWindow( ss.str().c_str() );
			glutKeyboardFunc( Key );
			glutDisplayFunc( Draw );
			glutMainLoop();
		}
	};

	int Generator( int x, int y ) restrict(cpu, amp) 
	{
		const float scale = 1.5f;
		const float half_dim = Julia::DIM / 2.f;
		
		ComplexNumber z(scale * (half_dim - x) / (half_dim), scale * (half_dim - y) / (half_dim));
		ComplexNumber c(-0.8f, 0.156f);
		//ComplexNumber c(-0.4f, 0.6f);
		
		for (int i = 0; i < 200; ++i)
		{
			z = z * z + c;
			if (z.magnitude2() > 1000)
				return 0;
		}

		return 1;
	}
};

#define GPU

int main()
{
	ShowWindow(GetConsoleWindow(), SW_HIDE);

	std::clock_t start = std::clock();

	#ifndef GPU
	//cpu
	for(uint32 i = 0; i < Julia::DIM; ++i)
		for(uint32 j = 0; j < Julia::DIM; ++j)
		{
			auto juliaValue = Julia::Generator(i, j);
			auto index = i + Julia::DIM * j;
			Julia::Buffer[index] = ((0x00 << 24) | ((0xFF * juliaValue) << 16) | ((0xFF * juliaValue) << 8) | 0xFF);
		}
	#else
	//gpu
	array_view<uint32, 2> jsbuffer_view(Julia::DIM, Julia::DIM, Julia::Buffer);
	
	parallel_for_each
	(
		jsbuffer_view.extent, 
		[=](index<2> idx) restrict (amp)
		{ 
			auto juliaValue = Julia::Generator(idx[1], idx[0]);
			jsbuffer_view[idx] = (0x00 << 24) | ((0xFF * juliaValue) << 16) | (0x00 << 8) | 0xFF;
		}
	);

	jsbuffer_view.synchronize();
	#endif

	double duration = (std::clock() - start) / static_cast<double>(CLOCKS_PER_SEC);

	const char* msg = 
	#ifndef GPU 
		"CPU"; 
	#else 
		"GPU"; 
	#endif

	Julia::App::Run(msg, duration);

	return 0;
}