//csc DllImport.cs Helper\DllImportHelper.cs
//csc DllImport.cs Helper\DllImportHelper.cs /fullpaths

using System;
using System.Runtime.InteropServices;

public class Win32APIHelper
{
  [DllImport("user32.dll", CharSet = CharSet.Ansi)]
  public static extern int MessageBoxA(int h, string m, string c, int mb);

  [DllImport("user32.dll", CharSet = CharSet.Unicode)]
  public static extern int MessageBoxW(int h, string m, string c, int mb);  
}

public class MinhaClasse
{
  public static void Main()
  {
    OperatingSystem op = Environment.OSVersion;
    Version v = op.Version;

    switch(op.Platform)
    {
      case PlatformID.Win32NT:
        Win32APIHelper.MessageBoxW(0, "NT " + v.ToString(), "OS Version:", 0);
        break;
      case PlatformID.Win32Windows:
        Win32APIHelper.MessageBoxA(0, "98 " + v.ToString(), "OS Version", 0);
        break;        
    }
  }
}
