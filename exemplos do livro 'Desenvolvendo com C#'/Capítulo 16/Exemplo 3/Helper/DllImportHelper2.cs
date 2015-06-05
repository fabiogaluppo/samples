using System.Runtime.InteropServices;

public class Win32APIHelper
{
  [DllImport("user32.dll", CharSet = CharSet.Ansi)]
  public static extern int MessageBoxA(int h, string m, string c, int mb);

  [DllImport("user32.dll", CharSet = CharSet.Unicode)]
  public static extern int MessageBoxW(int h, string m, string c, int mb);  
}