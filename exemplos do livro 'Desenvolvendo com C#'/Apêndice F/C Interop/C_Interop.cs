//csc C_Interop.cs

using System;
using System.Runtime.InteropServices;

public class CRT
{
  [DllImport("msvcrt.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
  public static extern int printf(string format, double d); 

  [DllImport("msvcrt.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
  public static extern int printf(string format, int i, string s); 

  [DllImport("msvcrt.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
  public static extern int strlen(string s);

  public static void Main()
  {
    CRT.printf("%f \n", 1000.2222d);

    string text = "The quick brown fox jumps over the lazy dog";

    CRT.printf("size = %d : %s", CRT.strlen(text), text); 
  }
}
