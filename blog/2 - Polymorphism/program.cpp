//cl /Za /Zi program.cpp

#include <cstdio>

const float PI = 3.14159265f;

class IConverter
{
public:
	virtual float Convert(float value) = 0;
};

class Radians2Degree : public IConverter
{
public:
	virtual float Convert(float value)
	{
		return 180 * (value / PI);
	}
};

class Degree2Radians : public IConverter
{
public:
	virtual float Convert(float value)
	{
		return PI * (value / 180);
	}
};

int main()
{
	Radians2Degree r2d;
	Degree2Radians d2r;
	
	IConverter* converter = &r2d;
	std::printf( "2PI in degrees == %f\n", converter->Convert(2 * PI) );

	converter = &d2r;
	std::printf( "360 in radians == %f\n", converter->Convert(360.f) );

	return 0;
}
