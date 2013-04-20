//Code provided by Fabio Galuppo 
//April 2012
//A homage to: 
//Logo (http://el.media.mit.edu/logo-foundation/logo/programming.html) and 
//Small Basic(http://msdn.microsoft.com/en-us/beginner/gg605166.aspx)

#include "mathhelper.hpp"

#include <cmath>

const float PI = 3.14159265f;

float degrees_to_radians(float angle)
{
	return angle * PI / 180.0f;
}

float radians_to_degrees(float angle)
{
	return angle * 180.0f / PI;
}

float lerp(float x0, float x1, float percent)
{
	return x0 + percent * (x1 - x0);
}

float clamp(float value, float max, float min)
{
	if(value > max) return max;
	else if (value < min) return min;
	return value;
}