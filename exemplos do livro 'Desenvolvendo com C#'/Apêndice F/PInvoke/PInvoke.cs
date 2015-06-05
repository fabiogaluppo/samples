//csc PInvoke.cs

using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct SMALL_RECT 
{
  public short Left; 
  public short Top; 
  public short Right; 
  public short Bottom; 
}

[StructLayout(LayoutKind.Sequential)]
public struct COORD 
{ 
  public short X; 
  public short Y; 
}  

[StructLayout(LayoutKind.Sequential)]
public struct CONSOLE_SCREEN_BUFFER_INFO 
{ 
  public COORD      dwSize; 
  public COORD      dwCursorPosition; 
  public int        wAttributes; 
  public SMALL_RECT srWindow; 
  public COORD      dwMaximumWindowSize; 
}

public class ConsoleUtil
{
    [DllImport("kernel32.dll")]
  public static extern IntPtr GetStdHandle(long nStdHandle);

  [DllImport("kernel32.dll")]
  public static extern bool GetConsoleScreenBufferInfo
  (
    IntPtr hConsole, 
    ref CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo
  );

  [DllImport("Kernel32.dll")]
  public static extern bool FillConsoleOutputCharacter
  (
    IntPtr hConsoleOutput,
    char cCharacter,               
    long nLength,                  
    COORD dwWriteCoord,             
    ref long lpNumberOfCharsWritten 
  );

  [DllImport("Kernel32.dll")]
  public static extern bool FillConsoleOutputAttribute
  (
    IntPtr hConsoleOutput,
    int wAttribute,
    long nLength,
    COORD dwWriteCoord,
    ref long lpNumberOfAttrsWritten
  );

  [DllImport("Kernel32.dll")]
  public static extern bool SetConsoleCursorPosition
  (
    IntPtr hConsoleOutput,
    COORD dwCursorPosition
  );

  [DllImport("kernel32.dll")]
  public static extern bool CloseHandle(IntPtr hObject);

  public const long STD_OUTPUT_HANDLE = -11;

  public static void CLS()
  {
    IntPtr hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
    
    CONSOLE_SCREEN_BUFFER_INFO csbi = new CONSOLE_SCREEN_BUFFER_INFO();  

    if(!GetConsoleScreenBufferInfo(hConsole, ref csbi))
      throw new ApplicationException("CLS não pode executado");
  
    long dwConSize = csbi.dwSize.X * csbi.dwSize.Y;

    COORD coordScreen = new COORD();
    coordScreen.Y = coordScreen.X = 0;

    long cCharsWritten = 0;

    if(FillConsoleOutputCharacter(hConsole, ' ', dwConSize, coordScreen, ref cCharsWritten))
      throw new ApplicationException("CLS não pode executado");

    if(!GetConsoleScreenBufferInfo(hConsole, ref csbi))
      throw new ApplicationException("Problema no reposicionamento1");
    
    if(FillConsoleOutputAttribute(hConsole, csbi.wAttributes, dwConSize, coordScreen, ref cCharsWritten))
      throw new ApplicationException("Problema no reposicionamento2");

    if(!SetConsoleCursorPosition(hConsole, coordScreen))
      throw new ApplicationException("Problema no reposicionamento3");

    if(!CloseHandle(hConsole))
      throw new ApplicationException("Handle não liberado");
  }
}

public class PInvoke
{
  public static void Main()
  {
    Console.WriteLine("Após o ENTER a tela irá se apagar");
    Console.ReadLine();
    ConsoleUtil.CLS();          
  }
}