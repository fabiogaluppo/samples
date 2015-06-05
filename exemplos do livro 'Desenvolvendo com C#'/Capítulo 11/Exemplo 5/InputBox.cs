//csc /t:library InputBox.cs /r:Microsoft.VisualBasic.dll

using Microsoft.VisualBasic;

namespace CSharp
{ 
  public class InputBox
  {  
    public static string Show(string prompt, string title)
    {
      return Interaction.InputBox(prompt, title, null, 0, 0);
    }  
  }
}