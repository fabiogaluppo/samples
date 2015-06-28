//Sample provided by Fabio Galuppo 
//June 2015 

//compile: csc /r:System.Core.dll /r:Microsoft.CSharp.dll /r:System.dll /r:%windir%\Microsoft.NET\Framework\v4.0.30319\WPF\PresentationCore.dll /r:%windir%\Microsoft.NET\Framework\v4.0.30319\WPF\PresentationFramework.dll /r:%windir%\Microsoft.NET\Framework\v4.0.30319\WPF\WindowsBase.dll /r:%windir%\Microsoft.NET\Framework\v4.0.30319\System.Xaml.dll /t:exe /out:bin\ClickSample.exe ClickSample.cs
//run: .\bin\ClickSample.exe

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

sealed class App : Application {
    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);
    }
}

sealed class ClickSample : Window {
    [STAThread]
    public static void Main(string[] args) {
        App app = new App();
        app.Run(new ClickSample());
    }
    
    public ClickSample() {
        Width = 420;
        Height = 240;
        Title = "'Reactive stream' with event click (aka event stream)";
        
        OnNextButton.Height = 25;
        OnNextButton.Width = 100;
        OnNextButton.Content = "OnNext";
        OnNextButton.Click += new RoutedEventHandler(OnNextButton_Clicked);
        
        OnCompletedButton.Height = 25;
        OnCompletedButton.Width = 100;
        OnCompletedButton.Content = "OnCompleted";
        OnCompletedButton.Click += new RoutedEventHandler(OnCompletedButton_Clicked);
        
        OnErrorButton.Height = 25;
        OnErrorButton.Width = 100;
        OnErrorButton.Content = "OnError";
        OnErrorButton.Click += new RoutedEventHandler(OnErrorButton_Clicked);
        
        SP.Children.Add(OnNextButton);
        SP.Children.Add(OnCompletedButton);
        SP.Children.Add(OnErrorButton);
        SP.Children.Add(Data);
        
        Content = SP;
    }
    
    private static string CurrentTime { get { return DateTime.Now.ToString("HH:mm:ss.fff"); } }
    
    private void OnNextButton_Clicked(object sender, RoutedEventArgs e) {
        Data.Items.Add(CurrentTime);
    }
    
    private void OnCompletedButton_Clicked(object sender, RoutedEventArgs e) {
        Application.Current.Shutdown();
    }
    
    private void OnErrorButton_Clicked(object sender, RoutedEventArgs e) {
        throw new Exception();
    }
    
    private StackPanel SP = new StackPanel();
    private Button OnNextButton = new Button();
    private Button OnCompletedButton = new Button();
    private Button OnErrorButton = new Button();
    private ListBox Data = new ListBox();
}