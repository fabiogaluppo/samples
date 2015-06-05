//csc /out:ConsoleSubscriber.exe /recurse:*.cs

using System;

public class App
{
  public static void Main()
  {
    Console.WriteLine("\nDigite a mensagem a ser publicada");
    Console.WriteLine("Mensagem vazia e ENTER para sair");
    Console.WriteLine("\nMensagem:");

    string mensagem = null;

    Subscriber c = new ConsoleSubscriber();
    Subscriber f = new FormSubscriber();
    Subscriber b = new BrowserSubscriber();

    Publisher.Broadcast broadcast = new Publisher.Broadcast(c.Receive);
    broadcast += new Publisher.Broadcast(f.Receive);
    broadcast += new Publisher.Broadcast(b.Receive);    

    do
    {      
      mensagem = Console.ReadLine();
      
      broadcast(mensagem);
    
    }while(mensagem.Trim() != String.Empty);
  }
}



