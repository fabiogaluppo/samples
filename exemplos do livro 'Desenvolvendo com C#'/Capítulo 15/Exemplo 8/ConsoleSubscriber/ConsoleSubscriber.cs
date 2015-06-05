//ConsoleSubscriber.cs

using System;

public class ConsoleSubscriber : Subscriber
{
  public override void Receive(string msg)
  {
    if(msg.Trim() != String.Empty)
      Console.WriteLine("--> {0} <--", msg);
  }  
}