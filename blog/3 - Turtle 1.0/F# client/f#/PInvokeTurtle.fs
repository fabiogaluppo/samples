//Code provided by Fabio Galuppo 
//April 2012
//A homage to: 
//Logo (http://el.media.mit.edu/logo-foundation/logo/programming.html) and 
//Small Basic(http://msdn.microsoft.com/en-us/beginner/gg605166.aspx)

namespace global

open System
open System.Runtime.InteropServices

module private API =
    [<DllImport(@"Turtle.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern nativeint create()
    [<DllImport(@"Turtle.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void rotate(nativeint hApp, float32 angle)
    [<DllImport(@"Turtle.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void resize(nativeint hApp, float32 size)
    [<DllImport(@"Turtle.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void move(nativeint hApp, int distance)
    [<DllImport(@"Turtle.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void speed(nativeint hApp, int distance)
    [<DllImport(@"Turtle.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void destroy(nativeint hApp)


[<Sealed>]
type private PInvokeTurtle() =
    let mutable disposed = false
    let mutable hTurtle = API.create()
       
    override this.Finalize() = this.close()

    member this.rotate(angle: float32) = API.rotate(hTurtle, angle)
   
    member this.resize(size: float32) = API.resize(hTurtle, size)
   
    member this.move(distance: int) = API.move(hTurtle, distance)
   
    member this.speed(value: int) = API.speed(hTurtle, value)

    member private this.close() = 
        if not disposed then 
            disposed <- true
            API.destroy(hTurtle)

    interface IDisposable with
        member this.Dispose() = GC.SuppressFinalize(this); this.close()