//Code provided by Fabio Galuppo 
//April 2012
//A homage to: 
//Logo (http://el.media.mit.edu/logo-foundation/logo/programming.html) and 
//Small Basic(http://msdn.microsoft.com/en-us/beginner/gg605166.aspx)

import java.nio.ByteBuffer;

final class JNITurtle
{
    private native ByteBuffer create();
    private native void rotate(ByteBuffer hTurtle, float angle);
    private native void resize(ByteBuffer hApp, float size);
    private native void move(ByteBuffer hApp, int distance);
    private native void speed(ByteBuffer hApp, int value);
    private native void destroy(ByteBuffer hApp);
    
    private boolean disposed;
    private ByteBuffer hTurtle;

    JNITurtle()
    {
        disposed = false;
        hTurtle = create();
    }

    static
    {
        System.loadLibrary("Turtle");
    }

    public void rotate(float angle)
    {
        rotate(hTurtle, angle);
    }

    public void resize(float size)
    {
        resize(hTurtle, size);
    }

    public void move(int distance)
    {
        move(hTurtle, distance);
    }

    public void speed(int value)
    {
        speed(hTurtle, value);
    }

    public void dispose()
    {
        if(!disposed)
        {
           disposed = true;
           destroy(hTurtle);
        }
    }

    protected void finalize()
    {
        dispose();
    }
}