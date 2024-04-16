using System;
using System.Collections.ObjectModel;
using LeagueTool.Models;
using LeagueTool.Services;
using ReactiveUI;

namespace LeagueTool.ViewModels;

public class ChampionViewModel : ViewModelBase
{
    private readonly ChampionService _championService;
    private bool _jungleFilterToggled = false;
    private bool _topFilterToggled = false;
    private bool _midFilterToggled = false;
    private bool _botFilterToggled = false;
    private bool _supportFilterToggled = false;
    private bool _allFalse = true;
    private string _filterText = "";


    public ChampionViewModel()
    {
        _championService = new ChampionService();
        var championList = _championService.GetChampionSaves();
        foreach (var champion in championList) ChampionList.Add(new ChampionListItemModel(champion));
    }
#pragma warning disable CA1822 // Mark members as static
    // public List<ChampionListItemModel> ChampionList => new ();

    public ObservableCollection<ChampionListItemModel> ChampionList { get; } = new();
    public bool FilterCompleted { get; set; }
    
    
    public bool JungleFilterToggled { 
        get => _jungleFilterToggled;
        set => this.RaiseAndSetIfChanged(ref _jungleFilterToggled, value);
    }

    public bool TopFilterToggled
    {
        get => _topFilterToggled;
        set => this.RaiseAndSetIfChanged(ref _topFilterToggled, value);
    }

    public bool MidFilterToggled { 
        get => _midFilterToggled;
        set => this.RaiseAndSetIfChanged(ref _midFilterToggled, value); 
    }
    
    public bool BotFilterToggled { 
        get => _botFilterToggled;
        set => this.RaiseAndSetIfChanged(ref _botFilterToggled, value); 
    }
    public bool SupportFilterToggled { 
        get => _supportFilterToggled;
        set => this.RaiseAndSetIfChanged(ref _supportFilterToggled, value); 
    }
    
    
    public bool AllFalse { 
        get => _allFalse;
        set => this.RaiseAndSetIfChanged(ref _allFalse, value); 
    }
    
#pragma warning restore CA1822 // Mark members as static
    public void ToggleShowCompletedFilter()
    {
        UpdateEntriesRegardingFilters();
    }

    public void UpdateEntriesRegardingFilters()
    {
        foreach (var champion in ChampionList)
        {
            if (_filterText != "" && !champion.Name.ToLower().Contains(_filterText.ToLower()))
            {
                champion.IsHidden = true;
                continue;
            }
            if (_filterText != "")
            {
                champion.IsHidden = false;
                continue;
            }
            
            bool isHidden = false;
            if (FilterCompleted)
            {
                isHidden = champion.Played;
            }
            if (TopFilterToggled && !champion.HasTopLane)
            {
                isHidden = true;
            }
            if (MidFilterToggled && !champion.HasMidLane)
            {
                isHidden = true;
            }
            if (BotFilterToggled && !champion.HasBotLane)
            {
                isHidden = true;
            }
            if (JungleFilterToggled && !champion.HasJungleLane)
            {
                isHidden = true;
            }
            if (SupportFilterToggled && !champion.HasSupportLane)
            {
                isHidden = true;
            }
            
            champion.IsHidden = isHidden;
        }
    }

    public void ToggleFilter(Lane lane)
    {
        Console.WriteLine(lane.ToString());
        switch (lane)
        {
            case Lane.Top:
                TopFilterToggled = TopFilterToggled && AllFalse || !TopFilterToggled;
                BotFilterToggled = false;
                MidFilterToggled = false;
                JungleFilterToggled = false;
                SupportFilterToggled = false;
                break;
            case Lane.Mid:
                TopFilterToggled = false;
                BotFilterToggled = false;
                MidFilterToggled = MidFilterToggled && AllFalse || !MidFilterToggled;
                SupportFilterToggled = false;
                JungleFilterToggled = false;
                break;
            case Lane.Jungle:
                TopFilterToggled = false;
                JungleFilterToggled = JungleFilterToggled && AllFalse || !JungleFilterToggled;
                MidFilterToggled = false;
                BotFilterToggled = false;
                SupportFilterToggled = false;
                break;
            case Lane.Bot:
                TopFilterToggled = false;
                JungleFilterToggled = false;
                MidFilterToggled = false;
                BotFilterToggled = BotFilterToggled && AllFalse || !BotFilterToggled;
                SupportFilterToggled = false;
                break;
            case Lane.Support:
                TopFilterToggled = false;
                JungleFilterToggled = false;
                MidFilterToggled = false;
                BotFilterToggled = false;
                SupportFilterToggled = SupportFilterToggled && AllFalse || !SupportFilterToggled;
                break;
        }
        
        AllFalse = !_topFilterToggled && !_midFilterToggled && !_botFilterToggled && !_jungleFilterToggled && !_supportFilterToggled;

        UpdateEntriesRegardingFilters();
    }

    public void FilterText(string? text)
    {
        _filterText = text ?? "";
        
        UpdateEntriesRegardingFilters();
    }
}