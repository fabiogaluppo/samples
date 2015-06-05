//csc PropPerf.cs /r:MesPerf.dll

using System;

public class Test
{
  #region Propriedade e seu armazenador interno
  private int number;
  
  public int NumberProp
  {
    get
    {
      return number;
    }

    set
    {
      number = value;
    }
  }
  #endregion

  #region Campo
  public int NumberField;
  #endregion

  public static void Prop(int size)
  {
    Test o = new Test();

    for(int a = 0; a < size; ++a)
    {
      o.NumberProp = a;
    }

    for(; 0 < o.NumberProp; --o.NumberProp);    
  }

  public static void Field(int size)
  {
    Test o = new Test();

    for(int a = 0; a < size; ++a)
    {
      o.NumberField = a;
    }

    for(; 0 < o.NumberField; --o.NumberField);    
  }

  public static void MesProp(int size)
  {
    Measure m = new Measure();

    m.Start();
   
    Prop(size);       

    m.Finish();

    Console.WriteLine("Prop ({1}) Tempo de processamento : {0}", m.Value, size);
  }

  public static void MesField(int size)
  {
    Measure m = new Measure();

    m.Start();
   
    Field(size);       

    m.Finish();

    Console.WriteLine("Campo ({1}) Tempo de processamento : {0}", m.Value, size);
  }

  public static void Main()
  {
    MesProp(1000000);
    MesField(1000000);

    MesProp(10000000);
    MesField(10000000);

    MesProp(100000000);
    MesField(100000000);

    MesProp(1000000000);
    MesField(1000000000);
  }
}
