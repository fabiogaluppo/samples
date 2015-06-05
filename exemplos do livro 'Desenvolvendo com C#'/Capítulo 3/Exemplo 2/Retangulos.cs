//csc Retangulos.cs /nologo

using System;

//Classe
public class CRetangulo
{
  //Campos
  public uint x, y;

  //C�lculo da �rea do ret�ngulo
  public ulong Area(){ return x * y; }
}

//Estrutura
public struct SRetangulo
{
  //Campos
  public uint x, y;

  //C�lculo da �rea do ret�ngulo
  public ulong Area(){ return x * y; }
}

public class Retangulos
{
  public static void Main()
  {
    CRetangulo cl = new CRetangulo(); //alocado na heap
    SRetangulo st;                    //alocado na stack

    cl.x = 10;
    cl.y = 5;

    st.x = 10;
    st.y = 5;

    Console.WriteLine("Classe - {0} m X {1} m = {2} m�", cl.x, cl.y, cl.Area());
    //Exibe na tela como na linha anterior, mas com excess�o do newline
    Console.Write("Estrutura - {0} m X {1} m = {2} m�", st.x, st.y, st.Area());
  }
}

