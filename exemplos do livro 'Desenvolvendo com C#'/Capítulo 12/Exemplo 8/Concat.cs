//csc Concat.cs

using System;
using System.Text;

public class Concat
{
  private StringBuilder _str;

  //Construtores

  public Concat(string s1, string s2) //c1
  {
    Initialize(s1, s2); 
  }

  public Concat(string s1, string s2, string s3) : this(s1,s2) //c2
  { 
    _str.Append(s3);    
  }

  public Concat(params string[] args) //c3
  {
    if(args.Length > 3)
    {
      //this(args[0], args[1], args[2]); //infelizmente não é permitido!
      
      Initialize(args[0], args[1]);
      for(int a = 2, l = args.Length; a < l; ++a)
      {
        _str.Append(args[a]); 
      }   
    } 
  }

  public Concat(object o1, object o2) //c4
  {
    Initialize(o1.ToString(), o2.ToString());
  }

  private void Initialize(string s1, string s2)
  {
    //A little trick :)
    (_str = new StringBuilder(s1)).Append(s2);
  }
  
  public override string ToString(){ return _str.ToString(); }    
}

public class Aplicação
{
  public static void Main()
  {
    //chama construtor c1
    Concat c1 = new Concat("123", "456");
    Console.WriteLine(c1.ToString());
 
    //chama construtor c2
    Concat c2 = new Concat("123", "456", "789");
    Console.WriteLine(c2.ToString()); 

    //chama construtor c3
    Concat c3 = new Concat("123", "456", "789", "0AB");
    Console.WriteLine(c3.ToString()); 

    //chama construtor c4
    Concat c4 = new Concat(123, 456);
    Console.WriteLine(c4.ToString()); 
  }
}
