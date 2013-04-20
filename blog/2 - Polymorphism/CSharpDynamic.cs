using System;

//Sample provided by Fabio Galuppo

class Program
{
    sealed class X 
    { 
        public String M(String value){ return value.ToUpper(); } 
    }

    sealed class Y 
    { 
        public String M(String value)
        { 
            char[] temp = value.ToCharArray();
            Array.Reverse(temp);
            return new String(temp); 
        }
    }

    sealed class Z : System.Dynamic.DynamicObject
    {
        Func<String, String> DefaultResult_ = s => String.Empty;
        
        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            result = DefaultResult_;
            return true;
        }
    }
    
    static String Selector(dynamic id, String value)
    {
        String result = String.Empty;
        
        try
        { 
            result = id.M(value); 
        }
        catch(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException){}

        return result;
    }

    static void Main(string[] args)
    {
        Action<dynamic, String> m = (id, value) => Console.WriteLine("{0} -> {1}", value, Selector(id, value));

        m(new X(), "Hello");
        m(new Y(), "World");
        m(new object(), "Something");        
        m(new Z(), "Something");

        Console.ReadLine();
    }
}