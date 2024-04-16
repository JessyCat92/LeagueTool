using System.Collections.Generic;
using LeagueTool.Services.Database;
using ReactiveUI;

namespace LeagueTool.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public enum PageViews
    {
        About,
        Champion
    }

    private readonly Dictionary<PageViews, ViewModelBase> _Pages;

    public MainWindowViewModel(MyDbContext myDbContext)
    {
        MyDb = myDbContext;

        _Pages = new Dictionary<PageViews, ViewModelBase>
        {
            { PageViews.About, new AboutViewModel() },
            { PageViews.Champion, new ChampionViewModel() }
        };

        _CurrentPage = _Pages[PageViews.Champion];
    }

    public ViewModelBase CurrentPage
    {
        get => _CurrentPage;
        private set => this.RaiseAndSetIfChanged(ref _CurrentPage, value);
    }

    public void SetCurrentPage(PageViews page)
    {
        CurrentPage = _Pages[page];
    }

    public void ReloadPage()
    {
        CurrentPage = _Pages[PageViews.About];
        _Pages[PageViews.Champion] = new ChampionViewModel();
        CurrentPage = _Pages[PageViews.Champion];
    }

#pragma warning disable CA1822 // Mark members as static
    private ViewModelBase _CurrentPage;
    public static MyDbContext? MyDb;
    // public object CurrentPage { get; }
#pragma warning restore CA1822 // Mark members as static
}