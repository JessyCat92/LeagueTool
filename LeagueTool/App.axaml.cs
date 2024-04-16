using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LeagueTool.Services.Database;
using LeagueTool.ViewModels;
using LeagueTool.Views;
using Microsoft.EntityFrameworkCore;

namespace LeagueTool;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var db = new MyDbContext();
            db.Database.Migrate();

            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(db)
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}