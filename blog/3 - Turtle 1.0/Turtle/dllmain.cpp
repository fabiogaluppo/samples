//Code provided by Fabio Galuppo 
//April 2012
//A homage to: 
//Logo (http://el.media.mit.edu/logo-foundation/logo/programming.html) and 
//Small Basic(http://msdn.microsoft.com/en-us/beginner/gg605166.aspx)

#include "scriptable.hpp"

extern "C"
{
	__declspec(dllexport) scriptable::TurtlePtr create()
	{
		return scriptable::create();
	}

	__declspec(dllexport) void rotate(scriptable::TurtlePtr hTurtle, float angle)
	{
		scriptable::rotate(hTurtle, angle);
	}

	__declspec(dllexport) void resize(scriptable::TurtlePtr hTurtle, float size)
	{
		scriptable::resize(hTurtle, size);
	}

	__declspec(dllexport) void move(scriptable::TurtlePtr hTurtle, int distance)
	{
		scriptable::move(hTurtle, distance);
	}

	__declspec(dllexport) void speed(scriptable::TurtlePtr hTurtle, int value)
	{
		scriptable::speed(hTurtle, value);
	}

	__declspec(dllexport) void destroy(scriptable::TurtlePtr hTurtle)
	{
		scriptable::destroy(hTurtle);
	}
}