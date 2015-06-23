//Sample provided by Fabio Galuppo 
//June 2015

//References:
//David W. Cantrell's Inverse gamma function (and Inverse factorial): http://mathforum.org/kb/message.jspa?messageID=342551&tstart=0
//DarkoVeberic's C++ implementation of the Lambert W(x) function: https://github.com/DarkoVeberic/LambertW

open System;;

open System.Runtime.InteropServices;;

[<DllImport("bin\\LambertW.dll", CallingConvention=CallingConvention.Cdecl)>]
extern float LambertW_0(float x);;

let c = 0.036534;;

let ln = Math.Log;;

let pi = Math.PI;;

let L x = ln((x + c) / Math.Sqrt(2. * pi));;

let W x = LambertW_0 x;;

let e = Math.E;;

let AIG x =
  //Approximated Inverse Gamma
  L(x) / (W (L(x) / e)) + 1. / 2.;;

let InvFact x =
  //Inverse Factorial in terms of rounded AIG
  Math.Round(AIG x) - 1.;;

//Tests:
let showAIG x =  
  printfn "AIG(%25.1f) = %9.6f" x (AIG x);;

let showInvFact x =  
  printfn "InvFact(%21.1f) = %9.6f" x (InvFact x);;

printfn "Inverse Gamma function and Inverse Factorial by Approximation";;

showAIG 1.;;

showAIG 24.;;

showAIG 362880.;;

showAIG 1.216451e+17;;

showInvFact 6.;;

showInvFact 24.;;

showInvFact 3628800.;;

showInvFact 2.432902e+18;;

(*

Inverse Gamma function and Inverse Factorial by Approximation
AIG(                      1.0) =  2.021203
AIG(                     24.0) =  4.994871
AIG(                 362880.0) =  9.998053
AIG(     121645100000000000.0) = 19.999281
InvFact(                  6.0) =  3.000000
InvFact(                 24.0) =  4.000000
InvFact(            3628800.0) = 10.000000
InvFact(2432902000000000000.0) = 20.000000

*)