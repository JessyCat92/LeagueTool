using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using LeagueTool.ViewModels;

namespace LeagueTool.Views;

public partial class AboutView : UserControl
{
    public AboutView()
    {
        InitializeComponent();
    }

    private void OpenUrl(object? sender, RoutedEventArgs e)
    {
        OpenUrl("https://github.com/JessyCat92");
    }

    public void OpenUrl(string urlObj)
    {
        var url = urlObj;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            //https://stackoverflow.com/a/2796367/241446
            using var proc = new Process { StartInfo = { UseShellExecute = true, FileName = url } };
            proc.Start();

            return;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("x-www-browser", url);
            return;
        }

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) throw new ArgumentException("invalid url: " + url);
        Process.Start("open", url);
    }

    private void ClickMain(object? sender, RoutedEventArgs e)
    {
        var context = Parent!.DataContext as MainWindowViewModel ??
                      throw new InvalidOperationException("DataContext is not MainWindowViewModel");

        context.SetCurrentPage(MainWindowViewModel.PageViews.Champion);
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        OpenUrl("https://github.com/JessyCat92/LeagueTool");
    }
}