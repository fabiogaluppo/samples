//Code provided by Fabio Galuppo 
//April 2012
//A homage to: 
//Logo (http://el.media.mit.edu/logo-foundation/logo/programming.html) and 
//Small Basic(http://msdn.microsoft.com/en-us/beginner/gg605166.aspx)

#include "scriptable.hpp"
#include "program.hpp"

#include <process.h>

unsigned int WINAPI Run(LPVOID instance)
{
	TurtleApp* app = reinterpret_cast<TurtleApp*>(instance);

	if (app != nullptr && SUCCEEDED(CoInitialize(NULL)) && SUCCEEDED(app->Initialize()))
	{
		app->Synchronize().Set();
		app->RunMessageLoop();
		
		delete app;

		CoUninitialize();
	}

	return 0;
}

scriptable::TurtlePtr scriptable::create()
{
	TurtleApp* app = new TurtleApp;

	_beginthreadex(NULL, 0, Run, app, 0, NULL);

	app->Synchronize().Wait();
	
	return reinterpret_cast<scriptable::TurtlePtr>(app);
}
	
void scriptable::rotate(scriptable::TurtlePtr hApp, float angle)
{
	auto app = reinterpret_cast<TurtleApp*>(hApp);
	app->Rotate(angle);
}

void scriptable::resize(scriptable::TurtlePtr hApp, float size)
{
	auto app = reinterpret_cast<TurtleApp*>(hApp);
	app->Resize(size);
}

void scriptable::move(scriptable::TurtlePtr hApp, int distance)
{
	auto app = reinterpret_cast<TurtleApp*>(hApp);	
	app->Move(distance);
}

void scriptable::speed(scriptable::TurtlePtr hApp, int value)
{
	auto app = reinterpret_cast<TurtleApp*>(hApp);	
	app->Speed(value);
}

void scriptable::destroy(scriptable::TurtlePtr hApp)
{
	auto app = reinterpret_cast<TurtleApp*>(hApp);
	app->Uninitialize();
}