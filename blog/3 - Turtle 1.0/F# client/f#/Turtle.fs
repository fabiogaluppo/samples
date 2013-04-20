//Code provided by Fabio Galuppo 
//April 2012
//A homage to: 
//Logo (http://el.media.mit.edu/logo-foundation/logo/programming.html) and 
//Small Basic(http://msdn.microsoft.com/en-us/beginner/gg605166.aspx)

namespace global

open System

type Turtle() =
   let turtle = new PInvokeTurtle()
   
   member this.close() = (turtle :> IDisposable).Dispose()
       
   member this.rotate(angle: float32) = turtle.rotate(angle)
   
   member this.resize(size: float32) = turtle.resize(size)
   
   member this.move(distance: int) = turtle.move(distance)
   
   member this.speed(value: int) = turtle.speed(value) 


