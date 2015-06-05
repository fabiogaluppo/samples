//csc DllImport.cs Helper\DllImportHelper.cs
//csc DllImport.cs Helper\DllImportHelper.cs /fullpaths

using System;

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
