using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using Avalonia.Threading;
using LeagueTool.Models;
using LeagueTool.Services.Database;
using LeagueTool.Services.Json;
using LeagueTool.ViewModels;

namespace LeagueTool.Services;

public class ChampionService
{
    private readonly MyDbContext _db;

    public ChampionService()
    {
        _db = MainWindowViewModel.MyDb!;

        // check if Update is needed by existing ConfigData with ConfigName = Version and ConfigName = LastUpdate and if championSaves is not empty
        if (_db.ConfigDatas.Any(x => x.ConfigName == "Version") && _db.ConfigDatas.Any(x => x.ConfigName == "LastUpdate") && _db.ChampionSaves.Any())
        {
            ConfigData? version = _db.ConfigDatas.FirstOrDefault(x => x.ConfigName == "Version");
            if (version != null)
            {
                if (version.ConfigValue == "0.0.0")
                {
                    UpdateChampionData();
                }
            }
            else
            {
                UpdateChampionData();
            }
        }
        else
        {
            Console.WriteLine("No Data found");
            UpdateChampionData();
        }
    }
    
    
    
    public void UpdateChampionData()
    {
        // check if cache directory exists if yes run FromCache otherwise use default
        if (Directory.Exists("Cache"))
        {
            UpdateChampionDataFromCache();
        }
        else
        {
            UpdateDatebaseToContainAllDefaultChampionData();
        }
    }

    public void UpdateChampionDataFromCache()
    {
        var championSaves = _db.ChampionSaves.ToList();

        // load all champions from champion.json from avalonia resources
        var text = File.ReadAllText("Cache/riot/dragontail/latest/data/en_US/champion.json");
        var parsedChampionJson = JsonSerializer.Deserialize<champion_json>(text)!;

        // loadChampionRates
        var textRates = File.ReadAllText("Cache/championRates.json");
        var parsedRatesData = JsonSerializer.Deserialize<rates_json>(textRates)!;

        // update database
        foreach (var champion in parsedChampionJson.data)
        {
            List<Lane> lanes = new();
            var ratesData = parsedRatesData.data.FirstOrDefault(x => x.Key == int.Parse(champion.Value.key));

            if (ratesData.Value.TOP.playRate > 0) lanes.Add(Lane.Top);
            if (ratesData.Value.JUNGLE.playRate > 0) lanes.Add(Lane.Jungle);
            if (ratesData.Value.MIDDLE.playRate > 0) lanes.Add(Lane.Mid);
            if (ratesData.Value.BOTTOM.playRate > 0) lanes.Add(Lane.Bot);
            if (ratesData.Value.UTILITY.playRate > 0) lanes.Add(Lane.Support);

            var championSave = championSaves.FirstOrDefault(x => x.ChampionKey.ToString() == champion.Value.key);
            if (championSave == null)
            {
                championSave = new ChampionSave
                {
                    ChampionKey = int.Parse(champion.Value.key),
                    ChampionName = champion.Value.name,
                    Version = champion.Value.version,
                    Played = false,
                    Tries = 0,
                    Lanes = lanes,
                    ImageName = champion.Value.image.GetValueOrDefault("full")!.ToString()!
                };
                _db.ChampionSaves.Add(championSave);
            }
            else
            {
                championSave.Version = champion.Value.version;
                championSave.ChampionName = champion.Value.name;
                championSave.Lanes = lanes;
                championSave.ImageName = champion.Value.image.GetValueOrDefault("full")!.ToString()!;
            }
        }

        _db.SaveChanges();
        
        Dispatcher.UIThread.Post(() =>
        {
            if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                (desktop.MainWindow?.DataContext as MainWindowViewModel)?.UpdateVersion();
                (desktop.MainWindow?.DataContext as MainWindowViewModel)?.UpdateChampions();
            }
        });
    }

    private void UpdateDatebaseToContainAllDefaultChampionData()
    {
        var championSaves = _db.ChampionSaves.ToList();

        // load all champions from champion.json from avalonia resources
        var championJson =
            AssetLoader.Open(new Uri("avares://LeagueTool/Assets/riot/dragontail/14.7.1/data/champion.json"));
        var reader = new StreamReader(championJson);
        var text = reader.ReadToEnd();
        var parsedChampionJson = JsonSerializer.Deserialize<champion_json>(text)!;

        // loadChampionRates
        var championRatesJson = AssetLoader.Open(new Uri("avares://LeagueTool/Assets/riot/championRates.json"));
        reader = new StreamReader(championRatesJson);
        var textRates = reader.ReadToEnd();
        var parsedRatesData = JsonSerializer.Deserialize<rates_json>(textRates)!;

        // update database
        foreach (var champion in parsedChampionJson.data)
        {
            List<Lane> lanes = new();
            var ratesData = parsedRatesData.data.FirstOrDefault(x => x.Key == int.Parse(champion.Value.key));

            if (ratesData.Value.TOP.playRate > 0) lanes.Add(Lane.Top);
            if (ratesData.Value.JUNGLE.playRate > 0) lanes.Add(Lane.Jungle);
            if (ratesData.Value.MIDDLE.playRate > 0) lanes.Add(Lane.Mid);
            if (ratesData.Value.BOTTOM.playRate > 0) lanes.Add(Lane.Bot);
            if (ratesData.Value.UTILITY.playRate > 0) lanes.Add(Lane.Support);

            var championSave = championSaves.FirstOrDefault(x => x.ChampionKey.ToString() == champion.Value.key);
            if (championSave == null)
            {
                championSave = new ChampionSave
                {
                    ChampionKey = int.Parse(champion.Value.key),
                    ChampionName = champion.Value.name,
                    Version = champion.Value.version,
                    Played = false,
                    Tries = 0,
                    Lanes = lanes,
                    ImageName = champion.Value.image.GetValueOrDefault("full")!.ToString()!
                };
                _db.ChampionSaves.Add(championSave);
            }
            else
            {
                championSave.Version = champion.Value.version;
                championSave.ChampionName = champion.Value.name;
                championSave.Lanes = lanes;
                championSave.ImageName = champion.Value.image.GetValueOrDefault("full")!.ToString()!;
            }
        }

        ConfigData? lastVersion = _db.ConfigDatas.FirstOrDefault(x => x.ConfigName == "LastUpdate");
        if (lastVersion != null)
        {
            lastVersion.ConfigValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        else
        {
            _db.ConfigDatas.Add(new ConfigData()
            {
                ConfigValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                ConfigName = "LastUpdate"
            });
        }
        ConfigData? version = _db.ConfigDatas.FirstOrDefault(x => x.ConfigName == "Version");
        if (version != null)
        {
            version.ConfigValue = "14.7.1";
        }
        else
        {
            _db.ConfigDatas.Add(new ConfigData()
            {
                ConfigValue = "14.7.1",
                ConfigName = "Version"
            });
        }

        _db.SaveChanges();
        
        if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            (desktop.MainWindow?.DataContext as MainWindowViewModel)?.UpdateVersion();
            (desktop.MainWindow?.DataContext as MainWindowViewModel)?.UpdateChampions();
        }
    }

    public List<ChampionSave> GetChampionSaves()
    {
        return _db.ChampionSaves.ToList();
    }

    public void ResetDatabase()
    {
        _db.ChampionSaves.RemoveRange(_db.ChampionSaves.ToList());
        _db.SaveChanges();
        UpdateChampionData();
    }
}