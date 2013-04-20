//Code provided by Fabio Galuppo 
//April 2012
//A homage to: 
//Logo (http://el.media.mit.edu/logo-foundation/logo/programming.html) and 
//Small Basic(http://msdn.microsoft.com/en-us/beginner/gg605166.aspx)

#include "turtle.hpp"

#include "mathhelper.hpp"

#include <vector>
#include <memory>

TurtleData make_TurtleData(float x, float y)
{
	TurtleData t = { x, y };
	return t;
}

Turtle::Turtle(float y, float x) : 
	m_DataPtr(new std::vector<TurtleData>()), 
	m_Step(100.0f), 
	m_AngleInRadians(0.0f),
	m_Speed(1)
{
	m_DataPtr->push_back( make_TurtleData(x, y) );
}

void Turtle::Move(int distance)
{
	using std::sin;
	using std::cos;

	auto angle = m_AngleInRadians;
	auto previous = m_DataPtr->at(m_DataPtr->size() - 1);
	auto x = previous.x + (distance * m_Step * cos(angle));
	auto y = previous.y + (distance * m_Step * sin(angle));
		
	m_DataPtr->push_back( make_TurtleData(x, y) );
}

void Turtle::Resize(float size)
{
	if(size > 0.1f)	m_Step = size;
}

Turtle::TurtleVector::const_iterator Turtle::Begin()
{
	return m_DataPtr->begin();
}

Turtle::TurtleVector::const_iterator Turtle::End()
{
	return m_DataPtr->end();
}

Turtle::TurtleVector::const_iterator Turtle::InitEnd()
{
	TurtleVector::const_iterator it = m_DataPtr->end();
	return --it;
}

void Turtle::Speed(int speed)
{
	m_Speed = static_cast<int>(clamp(static_cast<float>(speed), 10.0f, 1.0f));
}

void Turtle::Rotate(float angleInDegrees)
{
	float intPart = 0.0f, fracPart = 0.0f;
	fracPart = modf(angleInDegrees, &intPart);
	float newAngleInDegrees = static_cast<int>(intPart) % 360 + fracPart; 

	m_AngleInRadians += degrees_to_radians(-newAngleInDegrees);
}

const float Turtle::GetAngleInDegrees() const 
{ 
	return radians_to_degrees( m_AngleInRadians ); 
}