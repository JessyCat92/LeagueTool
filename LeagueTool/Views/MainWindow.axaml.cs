using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaDialogs.Views;
using LeagueTool.Services;
using LeagueTool.ViewModels;

namespace LeagueTool.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private MainWindowViewModel? ViewModel => (MainWindowViewModel?)DataContext;

    private void MenuItemAbout_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel!.SetCurrentPage(MainWindowViewModel.PageViews.About);
    }

    private void MenuItemExist_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void MenuItemReset_OnClick(object? sender, RoutedEventArgs e)
    {
        // MessageBox.ShowDialog("Some sample text", "Sample");
        TwofoldDialog dialog = new()
        {
            Message = "Are you sure you want to reset the database? This can not be undone!!!",
            NegativeText = "Cancel",
            PositiveText = "Start Reset",
            Background = new SolidColorBrush(Color.FromRgb(100, 0, 0))
        };
        var result = await dialog.ShowAsync();

        if (result.HasValue && result.Value)
        {
            var service = new ChampionService();
            service.ResetDatabase();

            ViewModel!.ReloadPage();
        }
    }
}