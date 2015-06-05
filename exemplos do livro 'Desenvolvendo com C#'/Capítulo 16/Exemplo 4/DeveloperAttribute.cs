//csc /t:library DeveloperAttribute.cs

using System;

[AttributeUsage(AttributeTargets.All)]
public class DeveloperAttribute : Attribute 
{ 
  public DeveloperAttribute(string name)
  { 
    m_Name = name;
  } 

  public string Name 
  {
    get{ return m_Name;  }
    set{ m_Name = value; }
  }

  public CodeStatus CodeStatus 
  {
    get{ return m_CodeStatus;  }
    set{ m_CodeStatus = value; }  
  }
    
  private string m_Name;
  private CodeStatus m_CodeStatus;
} 

public enum CodeStatus
{
  NotImplemented,
  ToReview,
  Freezed
}