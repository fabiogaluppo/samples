//Code provided by Fabio Galuppo 
//April 2012
//A homage to: 
//Logo (http://el.media.mit.edu/logo-foundation/logo/programming.html) and 
//Small Basic(http://msdn.microsoft.com/en-us/beginner/gg605166.aspx)

#pragma once

namespace scriptable
{
	typedef unsigned int* TurtlePtr;

	TurtlePtr create();	
	void rotate(TurtlePtr hApp, float angle);	
	void resize(TurtlePtr hApp, float size);
	void speed(TurtlePtr hApp, int value);
	void move(TurtlePtr hApp, int distance);
	void destroy(TurtlePtr hApp);
}