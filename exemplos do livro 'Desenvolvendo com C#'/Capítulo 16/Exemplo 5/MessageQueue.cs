//csc /t:library MessageQueue /r:DeveloperAttribute.dll 
//csc MessageQueue /r:DeveloperAttribute.dll /d:TEST

using System;
using System.Diagnostics;

[Developer("Fabio Galuppo", CodeStatus = CodeStatus.ToReview)]
public class MessageQueue
{
  [Developer("Fabio Galuppo", CodeStatus = CodeStatus.ToReview)]
  string[] queue;  

  [Developer("Fabio Galuppo", CodeStatus = CodeStatus.Freezed)]
  int idx;

  [Developer("Fabio Galuppo", CodeStatus = CodeStatus.ToReview)]
  public MessageQueue(int size)
  {
    queue = new string[size];
    idx = 0;
  }

  [Developer("Fabio Galuppo", CodeStatus = CodeStatus.Freezed)]
  public void Enqueue(string msg)
  {
    if(!IsFull)
      queue[idx++] = msg;
  }
  
  [Developer("Fabio Galuppo", CodeStatus = CodeStatus.ToReview)]
  public string Dequeue()
  {
    if(idx > 0)
    {
      string msgtmp = queue[queue.Length - idx];
      idx--;
      return msgtmp;
    }
    throw new ApplicationException("Queue is empty");
  }

  [Developer("Wallace Santos", CodeStatus = CodeStatus.NotImplemented)]
  public string Peek()
  {
    throw new NotImplementedException();
  }

  [Developer("Wallace Santos", CodeStatus = CodeStatus.Freezed)]
  public bool IsFull
  {
    get{ return idx == queue.Length; }  
  }

  [Developer("Vanclei Matheus", CodeStatus = CodeStatus.Freezed)]
  public int Count
  {
    get{ return idx; }
  }

  [Conditional("TEST")]
  private static void Main()
  {
    MessageQueue mq = new MessageQueue(10);

    for(int a = 0; a < 10; a++)
      mq.Enqueue("Msg" + (a + 1).ToString());

    if(mq.IsFull)
      for(int a = 0, l = mq.Count; a < l; a++)
         Console.WriteLine(mq.Dequeue());     
  }
}