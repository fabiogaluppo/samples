//csc Interception.cs

using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Activation;

public sealed class InterceptAspect : IMessageSink 
{
  private IMessageSink m_Next;
	
  public InterceptAspect(IMessageSink next) 
  { 
    m_Next = next;
  }

  public IMessageSink NextSink 
  {
    get 
    {
      return m_Next;
    }
  }
    
  public IMessage SyncProcessMessage(IMessage msg) 
  {
    PreProcess(msg);
			
    IMessage returnMethod = m_Next.SyncProcessMessage(msg);			
			
    PostProcess(msg, returnMethod);
			
    return returnMethod;
  }
    
  public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink) 
  {
    return m_Next.AsyncProcessMessage(msg, replySink); 	
  }
    
  private void PreProcess(IMessage msg) 
  {
    Console.WriteLine("Pré-processamento...");
  }

  private void PostProcess(IMessage msg, IMessage msgReturn)
  {    
    Console.WriteLine("Pós-processamento...");
  }
}

public sealed class InterceptProperty : IContextProperty, IContributeObjectSink 
{
  public IMessageSink GetObjectSink(MarshalByRefObject o, IMessageSink next) 
  {
    return new InterceptAspect(next);
  }

  public bool IsNewContextOK( Context newCtx ) 
  {
    return true;
  }
		
  public void Freeze(Context newCtx)
  {
    return;
  }
		
  public string Name 
  {
    get 
    {
      return "InterceptProperty";
    }
  }
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class InterceptAttribute : ContextAttribute 
{
  public InterceptAttribute() : base("InterceptAttribute"){}
		
  public override void GetPropertiesForNewContext(IConstructionCallMessage ccm) 
  {
    ccm.ContextProperties.Add(new InterceptProperty());
  }
}
	
[Intercept]
public class MinhaClasse : ContextBoundObject
{
  private void Método()
  {
    Console.WriteLine("Método Executado");
  }
  
  public static void Main()
  { 
    MinhaClasse mc = new MinhaClasse();
    
    mc.Método();  
  }
}
