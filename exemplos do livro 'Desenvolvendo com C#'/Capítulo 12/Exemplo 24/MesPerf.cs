//MesPerf.cs

using System;

internal class Measure
{
  DateTime begin, end;
  bool measuring = false, computed = false;

  internal void Start()
  {
    Check();

    computed = false;
    measuring = true;
    begin = DateTime.Now;
  }

  internal void Finish()
  {
    if(measuring)
    {
      end = DateTime.Now;
      measuring = false;
      computed = true;       
    }    
  }
  
  internal TimeSpan Value
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
    
  internal bool IsMeasuring
  {
    get
    {
      return measuring;     
    }
  }  
}
 
