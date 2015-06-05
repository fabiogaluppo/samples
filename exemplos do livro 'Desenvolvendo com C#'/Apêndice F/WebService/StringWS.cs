//csc /t:library /out:bin\StringWS.dll StringWS.cs

using System;
using System.Web.Services;

[WebService(Namespace = "http://www.desenvolvendo.net")]
public class StringWebService 
{
  [WebMethod]
  public int NumberOfVowels(string text)
  {
    int count = 0;

    for(int a = 0, l = text.Length; a < l; ++a)
    {
      switch(text[a])
      {
        case 'a':
        case 'e':
        case 'i':
        case 'o':
        case 'u':
          count++;
          break;
      }
    }
	
    return count;
  }
}