using System.Collections.Generic;
using System.Linq;
using LeagueTool.Services.Database;
using Microsoft.EntityFrameworkCore;
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
        
        UpdateVersion();
    }

    public void UpdateVersion()
    {
        if (MyDb == null) return;
        
        // query ConfigData with ConfigName = Version and ConfigName = LastUpdate and merge their values to a string
        MyDb!.ConfigDatas.Where(x => x.ConfigName == "Version" || x.ConfigName == "LastUpdate").ToListAsync().ContinueWith(result =>
        {
            var version = result.Result.FirstOrDefault(x => x.ConfigName == "Version")?.ConfigValue ?? "14.7.1";
            var lastUpdate = result.Result.FirstOrDefault(x => x.ConfigName == "LastUpdate")?.ConfigValue ?? "unknown";

            VersionText = $"Riot Patch: {version} | Last Update: {lastUpdate}";
        });
    }

    public ViewModelBase CurrentPage
    {
        get => _CurrentPage;
        private set => this.RaiseAndSetIfChanged(ref _CurrentPage, value);
    }

    public bool CloseOnClickAway
    {
        get => _closeOnClickAway;
        set => this.RaiseAndSetIfChanged(ref _closeOnClickAway, value); 
    }

    public string VersionText { 
        get => _versionText; 
        set => this.RaiseAndSetIfChanged(ref _versionText, value); 
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
    private bool _closeOnClickAway = true;
    private string _versionText = "Riot Patch: unknown";
    // public object CurrentPage { get; }
#pragma warning restore CA1822 // Mark members as static
}