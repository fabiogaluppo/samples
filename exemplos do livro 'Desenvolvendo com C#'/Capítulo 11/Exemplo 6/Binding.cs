//csc Binding.cs

public class GenericUser
{
  internal bool status; 

  public string Password;
  
  public bool IsAutenticated
  {
    get{ return status; }
  } 
}

public class WindowsUser : GenericUser
{
  public string Username;
}

public class PassportUser : GenericUser
{
  public string Email;
}

public class Authentication
{
  public static void Logon(GenericUser user)
  {
    user.status = true; //static binding   
  }

  public static void Logoff(GenericUser user)
  {
    user.status = false; //static binding
  }
}

public class Biding
{
  public static void Main()
  {
    WindowsUser wu = new WindowsUser();
    wu.Username = "fabiog";

    Authentication.Logon(wu); //dynamic binding

    PassportUser pu = new PassportUser();
    pu.Email = "fabiog@desenvolvendo.net";

    Authentication.Logon(pu); //dynamic binding  
  }
}