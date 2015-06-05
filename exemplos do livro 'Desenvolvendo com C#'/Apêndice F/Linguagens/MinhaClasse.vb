'vbc /t:library /out:MinhaClasseVB.dll MinhaClasse.vb /r:MinhaClasse.dll

Public Class MinhaClasseVB 
  Inherits MinhaClasseCS

  Public Sub Metodo2() 
  
    System.Console.WriteLine("VB.NET")
  
  End Sub

End Class