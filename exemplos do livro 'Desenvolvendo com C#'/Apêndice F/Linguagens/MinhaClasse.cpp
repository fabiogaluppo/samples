//cl /LD /CLR /FeMinhaClasseCPP.dll MinhaClasse.cpp

#using <mscorlib.dll>
#using <MinhaClasseCS.dll>
#using <MinhaClasseVB.dll>

__gc public class MinhaClasseCPP : public MinhaClasseVB
{

public: 
  void Metodo3()
  {
    System::Console::WriteLine("VC++.NET");
  }
};