//WinApiHelper.cs

using System.Runtime.InteropServices;

public sealed class WinAPIHelper
{  
  [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
  internal static extern short GlobalAddAtom(
    string lpString
  );

  [DllImport("kernel32.dll")]
  internal static extern short GlobalDeleteAtom(
    short nAtom
  );

  [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
  internal static extern uint GlobalGetAtomName(
    short nAtom,
    char[] lpBuffer,
    int nSize
  );
}