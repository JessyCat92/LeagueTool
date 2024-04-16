using System;
using System.Collections.Generic;
using System.Reactive;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using LeagueTool.Models;
using ReactiveUI;

namespace LeagueTool.ViewModels;

public class ChampionListItemModel : ReactiveObject
{
    private object _bgColor;
    private bool _played;
    private int _tries;
    private bool _isHidden = false;

    public ChampionListItemModel(ChampionSave championSave)
    {
        Name = championSave.ChampionName;
        Key = championSave.ChampionKey;
        Lanes = championSave.Lanes;
        Played = championSave.Played;
        Tries = championSave.Tries;
        BackgroundColor = Played ? Brushes.DarkGreen : Brushes.Transparent;
        PicturePath = $"avares://LeagueTool/Assets/riot/dragontail/14.7.1/champion/{championSave.ImageName}";

        try
        {
            // Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            Picture = new Bitmap(AssetLoader.Open(new Uri(PicturePath)));
            // Picture = new Bitmap(PicturePath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        MarkDone = ReactiveCommand.Create(() =>
        {
            Played = true;
            BackgroundColor = Brushes.DarkGreen;

            championSave.Played = true;

            MainWindowViewModel.MyDb!.SaveChanges();
        });

        AddTry = ReactiveCommand.Create(() =>
        {
            Tries++;
            championSave.Tries++;

            MainWindowViewModel.MyDb!.SaveChanges();
        });
    }

#pragma warning disable CA1822 // Mark members as static
    public string Name { get; set; } = "Placeholder";

    public int Key { get; set; }

    public List<Lane> Lanes { get; set; } = new();

    public bool Played
    {
        get => _played;
        set => this.RaiseAndSetIfChanged(ref _played, value);
    }

    public int Tries
    {
        get => _tries;
        set => this.RaiseAndSetIfChanged(ref _tries, value);
    }

    public string PicturePath { get; set; }
    public Bitmap Picture { get; set; }
    public bool HasTopLane => Lanes.Contains(Lane.Top);
    public bool HasJungleLane => Lanes.Contains(Lane.Jungle);
    public bool HasMidLane => Lanes.Contains(Lane.Mid);

    public bool IsHidden { 
        get => _isHidden;
        set => this.RaiseAndSetIfChanged(ref _isHidden, value); 
    }
    
    public bool HasBotLane => Lanes.Contains(Lane.Bot);
    public bool HasSupportLane => Lanes.Contains(Lane.Support);
    public ReactiveCommand<Unit, Unit> MarkDone { get; set; }
    public ReactiveCommand<Unit, Unit> AddTry { get; set; }

    public object BackgroundColor
    {
        get => _bgColor;
        set => this.RaiseAndSetIfChanged(ref _bgColor, value);
    }

#pragma warning restore CA1822 // Mark members as static
}