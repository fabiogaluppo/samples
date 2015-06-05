//csc @respfile.rsp

using System;

public class Matemática
{
  static void PerformGen(int tamanho)
  {
    Measure m = new Measure(); //classe helper para medição
    MatGen g = new MatGen();   //classe genérica
  
    object[] o = new object[tamanho];
    for(int a = 0; a < tamanho; ++a) o[a] = (double)a;      

    Console.WriteLine("Genérica {0} elementos", o.Length);
    
    m.Start();  
  
    g.Soma(typeof(double), o);

    m.Finish(); 
   
    Console.WriteLine("Tempo de processamento : {0}", m.Value);    
  }
  
  static void PerformEsp(int tamanho)
  {
    Measure m = new Measure(); //classe helper para medição
    MatEsp e = new MatEsp();   //classe específica
    
    double[] d = new double[tamanho];
    for(int a = 0; a < tamanho; ++a) d[a] = (double)a;

    Console.WriteLine("Específica {0} elementos", d.Length);
    
    m.Start();  
  
    e.Soma(d);

    m.Finish(); 
   
    Console.WriteLine("Tempo de processamento : {0}", m.Value);
  } 

  public static void Main()
  {
    //As medidas apresentadas neste exemplo
    //desprezam a montagem do vetor interno
    
    PerformEsp(1000);
    PerformGen(1000);    
    
    PerformEsp(10000);
    PerformGen(10000);
    
    PerformEsp(100000);
    PerformGen(100000);
    
    PerformEsp(1000000);
    PerformGen(1000000);
    
    PerformEsp(10000000);
    PerformGen(10000000);
  }
}