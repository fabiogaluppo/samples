using System;

public class ExcelAutomation
{
  public static void Main()
  {
    Excel.Application ex = new Excel.Application();
    ex.Visible = true; 
    Excel.Workbook wb = ex.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);
			
    Excel.Range rg = ex.ActiveCell;
    
    rg.set_Item(1, 1, "Competência");
    rg.set_Item(1, 2, "Nota");
    rg.set_Item(2, 1, "C++");
    rg.set_Item(2, 2, "5");
    rg.set_Item(3, 1, "UML");
    rg.set_Item(3, 2, "6");
    rg.set_Item(4, 1, "OOP");
    rg.set_Item(4, 2, "5");
    rg.set_Item(5, 1, "DB");
    rg.set_Item(5, 2, "9");  
    rg.set_Item(6, 1, ".NET");
    rg.set_Item(6, 2, "4");  
    rg.set_Item(7, 1, "XML");
    rg.set_Item(7, 2, "4");

    rg.AutoFormat(Excel.XlRangeAutoFormat.xlRangeAutoFormatClassic3, true, true, true, true, true, true);

    Excel.Chart ch = (Excel.Chart)ex.Charts.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
    ch.ChartType = Excel.XlChartType.xlRadarFilled;
			
    Excel.Worksheet ws = (Excel.Worksheet)ex.Worksheets["Plan1"];
			
    Excel.Range lrg = ws.get_Range("A1","B7");

    ch.SetSourceData(lrg, Excel.XlRowCol.xlColumns);
    ch.Location(Excel.XlChartLocation.xlLocationAsObject, "Plan1");

    ex.ActiveChart.HasLegend = false;
    ex.ActiveChart.HasTitle = true;
    ex.ActiveChart.ChartTitle.Text = "Gráfico de Competência";

    Console.Write("Pressionar ENTER para sair do EXCEL"); Console.ReadLine();
			
    ex.Quit();
  }
}