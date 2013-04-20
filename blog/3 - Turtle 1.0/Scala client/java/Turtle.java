//Code provided by Fabio Galuppo 
//April 2012
//A homage to: 
//Logo (http://el.media.mit.edu/logo-foundation/logo/programming.html) and 
//Small Basic(http://msdn.microsoft.com/en-us/beginner/gg605166.aspx)

public class Turtle
{
    private JNITurtle turtle;

    public Turtle()
    {
        turtle = new JNITurtle();
    }

    public void close()
    {
        turtle.dispose();
    }

    public void rotate(float angle)
    {
        turtle.rotate(angle);
    }

    public void resize(float size)
    {
        turtle.resize(size);
    }

    public void move(int distance)
    {
        turtle.move(distance);
    }

    public void speed(int value)
    {
        turtle.speed(value);
    }
}