//csc Musicians.cs /warn:0

public enum Instrument
{
  Bass,
  EletricGuitar,
  AcousticGuitar
}

public class Musician
{
  private string Name;
  private Instrument Instrument;
  
  public Musician(string name, Instrument instrument)
  {
    Name = name; Instrument = instrument;
  }  

  public static void Play(Musician m)
  {
    System.Console.WriteLine("{0} plays {1}", m.Name, m.Instrument.ToString());
  }
}

public class ProfessionalMusician : Musician
{
  public ProfessionalMusician(string name, Instrument instrument) : base(name, instrument)
  {    
  }
  
  private object[] Styles;
}

public class Musicians
{
  public static void Main()
  {
    Musician m = new Musician("Vanclei Matheus", Instrument.Bass);
    
    ProfessionalMusician pm; 
    pm = new ProfessionalMusician("John Petrucci", Instrument.EletricGuitar);

    Musician.Play(m);
    Musician.Play(pm); 
  }
}
