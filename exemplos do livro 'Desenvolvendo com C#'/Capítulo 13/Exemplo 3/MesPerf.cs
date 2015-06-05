//csc /t:library MesPerf.cs

using System;

public class Measure
{
  DateTime begin, end;
  bool measuring = false, computed = false;

  public void Start()
  {
    Check();

    computed = false;
    measuring = true;
    begin = DateTime.Now;
  }

  public void Finish()
  {
    if(measuring)
    {
      end = DateTime.Now;
      measuring = false;
      computed = true;       
    }    
  }
  
  public TimeSpan Value
  {
    get
    {
      Check(); 

      if(computed)
        return end - begin;
      throw new Exception("Cannot be performed in computing state");      
    }
  }

  private void Check()
  {
    if(measuring) 
      throw new Exception("Cannot be performed in measuring state");
  }
    
  public bool IsMeasuring
  {
    get
    {
      return measuring;     
    }
  }  
}
 
