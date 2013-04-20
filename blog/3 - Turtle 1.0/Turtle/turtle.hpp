//Code provided by Fabio Galuppo 
//April 2012
//A homage to: 
//Logo (http://el.media.mit.edu/logo-foundation/logo/programming.html) and 
//Small Basic(http://msdn.microsoft.com/en-us/beginner/gg605166.aspx)

#pragma once

#include <vector>
#include <memory>

struct TurtleData
{
	float x, y;
};

//Model
class Turtle
{
	typedef std::vector<TurtleData> TurtleVector;
	
public:
	typedef TurtleVector::const_iterator TurtleVectorConstIterator;

public:
	Turtle(float y = 0.0f, float x = 0.0f);

	void Move(int distance = 1);

	void Speed(int speed = 1);

	void Resize(float size);

	TurtleVector::const_iterator Begin();

	TurtleVector::const_iterator End();
	
	TurtleVector::const_iterator InitEnd();
	
	 void Rotate(float angleInDegrees);

	const float GetAngleInDegrees() const;
	
	const float GetSize() const { return m_Step; }

	const int GetSpeed() const { return m_Speed; }

private:
	std::unique_ptr<TurtleVector> m_DataPtr;
	float m_Step, m_AngleInRadians;
	int m_Speed;
};