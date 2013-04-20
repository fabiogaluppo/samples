//Code provided by Fabio Galuppo 
//April 2012
//A homage to: 
//Logo (http://el.media.mit.edu/logo-foundation/logo/programming.html) and 
//Small Basic(http://msdn.microsoft.com/en-us/beginner/gg605166.aspx)

#include "scriptable.hpp"

#include <jni.h>

extern "C" 
{
	JNIEXPORT jobject JNICALL Java_JNITurtle_create(JNIEnv* env, jobject)
	{
		return env->NewDirectByteBuffer(scriptable::create(), 0);
	}

	JNIEXPORT void JNICALL Java_JNITurtle_rotate(JNIEnv* env, jobject, jobject hTurtle, jfloat angle)
	{
		auto turtlePtr = reinterpret_cast<scriptable::TurtlePtr>(env->GetDirectBufferAddress(hTurtle));
		scriptable::rotate(turtlePtr, angle);
	}

	JNIEXPORT void JNICALL Java_JNITurtle_resize(JNIEnv* env, jobject, jobject hTurtle, jfloat size)
	{
		auto turtlePtr = reinterpret_cast<scriptable::TurtlePtr>(env->GetDirectBufferAddress(hTurtle));
		scriptable::resize(turtlePtr, size);
	}
	
	JNIEXPORT void JNICALL Java_JNITurtle_move(JNIEnv* env, jobject, jobject hTurtle, jint distance)
	{
		auto turtlePtr = reinterpret_cast<scriptable::TurtlePtr>(env->GetDirectBufferAddress(hTurtle));
		scriptable::move(turtlePtr, distance);
	}

	JNIEXPORT void JNICALL Java_JNITurtle_speed(JNIEnv* env, jobject, jobject hTurtle, jint speed)
	{
		auto turtlePtr = reinterpret_cast<scriptable::TurtlePtr>(env->GetDirectBufferAddress(hTurtle));
		scriptable::speed(turtlePtr, speed);
	}

	JNIEXPORT void JNICALL Java_JNITurtle_destroy(JNIEnv* env, jobject, jobject hTurtle)
	{
		auto turtlePtr = reinterpret_cast<scriptable::TurtlePtr>(env->GetDirectBufferAddress(hTurtle));
		scriptable::destroy(turtlePtr);
	}
}