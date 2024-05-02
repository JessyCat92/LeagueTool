using System;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaDialogs.Views;
using LeagueTool.Services.Progress;
using LeagueTool.ViewModels;

namespace LeagueTool.Views;

public partial class ProgressBarDialogView : BaseDialog<bool>
{
    // public ProgressBarDialog(Progress<ProgressState> progress)
    // {
    //     InitializeComponent();
    //     
    //     // progress.ProgressChanged += ProgressChanged;
    // }

    public ProgressBarDialogView()
    {
        InitializeComponent();
        // AvaloniaXamlLoader.Load(this);
        DataContext = new ProgressBarDialogViewModel();
    }
    
    public ProgressBarDialogView(Progress<ProgressState> progress)
    {
        InitializeComponent();
        // AvaloniaXamlLoader.Load(this);
        DataContext = new ProgressBarDialogViewModel();
        
        progress.ProgressChanged += ProgressChanged;
    }

    private void ProgressChanged(object? sender, ProgressState e)
    {
        // Console.WriteLine(DataContext);
        
        if (DataContext == null)
        {
            Console.WriteLine("Could not update progress, ViewModel is null.");
            return;
        }
        //
        ProgressBarDialogViewModel context = (ProgressBarDialogViewModel)DataContext;
        context.CurrentStep = e.State;
        context.CurrentProgress = e.Progress;
        if (e.Progress >= 100)
        {
            context.IsFinished = true;
        }
    }


    private void ButtonConfirm_Click(object? sender, RoutedEventArgs e)
    {
        Close(true);
    }
}
