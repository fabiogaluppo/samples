//Sample provided by Fabio Galuppo 
//June 2015

//compile: cl /LD /Fo.\obj\ /Fe.\bin\ /Ox LambertW.cc dll_program.cpp

#include "LambertW.h"

extern "C" double __declspec(dllexport) LambertW_0(double x)
{
	return utl::LambertW(0, x);
}

#include <windows.h>

BOOL WINAPI DllMain(HINSTANCE, DWORD fdwReason, LPVOID)
{
	switch (fdwReason)
	{
	case DLL_PROCESS_ATTACH:
		break;
	case DLL_THREAD_ATTACH:
		break;
	case DLL_THREAD_DETACH:
		break;
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}