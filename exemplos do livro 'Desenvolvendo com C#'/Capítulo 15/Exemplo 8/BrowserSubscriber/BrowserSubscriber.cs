using System;
using System.Text;
using System.Threading;
using System.Net.Sockets;

public class BrowserSubscriber : Subscriber
{
  public override void Receive(string msg)
  {
    if(msg.Trim() != String.Empty) 
      SendMessageToBrowser(msg);    
  }

  public void Browser()
  {
    System.Diagnostics.Process.Start("IExplore.exe", "http://localhost:1234");
  }
  
  private string Data(string content)
  {
    string term = "\r\n";
    string title = "<HEAD><TITLE>BrowserSubscriber</TITLE></HEAD>";
    string fmt = "<HTML>{1}<BODY><H1>{0}</H1></BODY></HTML>";
    string html = String.Format(fmt, content, title);
    
    StringBuilder resp = new StringBuilder(200);
    resp.AppendFormat("HTTP/1.1 200 OK{0}", term);
    resp.AppendFormat("Content-Type: text/html{0}", term);
    resp.AppendFormat("Content-Length: {0}{1}", html.Length, term);
    resp.AppendFormat("{0}{1}", term, html);

    return resp.ToString();
  }

  private void SendMessageToBrowser(string msg)
  {  
    ThreadStart ts = new ThreadStart(Browser);
    Thread t = new Thread(ts);  

    int port = 1234;
    TcpListener listener = new TcpListener(port);

    listener.Start();

    t.Start();
    
    TcpClient client = listener.AcceptTcpClient();
     
    NetworkStream ns = client.GetStream();

    string data = Data(msg);     

    Byte[] bytes = Encoding.ASCII.GetBytes(data);  
    ns.Write(bytes, 0, bytes.Length);

    client.Close();
    listener.Stop();   
  }
}