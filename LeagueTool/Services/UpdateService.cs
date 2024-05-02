using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Tar;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LeagueTool.Models;
using LeagueTool.Services.Database;
using LeagueTool.Services.Progress;
using LeagueTool.Services.Utils;
using LeagueTool.ViewModels;
using SevenZipExtractor;

namespace LeagueTool.Services;

public class UpdateService(IProgress<ProgressState> progress)
{
    enum UpdateState
    {
        Initializing,
        CheckingAssetVersion,
        BackupOldAssets,
        DownloadingAssets,
        ExtractingAssetsStep1,
        ExtractingAssetsStep2,
        ApplyingAssets,
        CleanUpDownload,
        ReceivingChampionStatistics,
        CleanUp,
        ApplyingNewVersion,
        RollingBack,
        Finished
    }
    
    private int _allSteps = Enum.GetValues<UpdateState>().Length;
    private UpdateState _currentState = UpdateState.Initializing;
    private MyDbContext _db;

    public async Task StartUpdate()
    {
        SetUpdateStep(UpdateState.Initializing);
        _db = MainWindowViewModel.MyDb!;
        
        SetUpdateStep(UpdateState.CheckingAssetVersion);
        Version? assetUpdateAvailable = await IsAssetUpdateAvailable();

        try
        {
            if (assetUpdateAvailable != null)
            {
                // Do Full Update Process
                await StartAssetUpdate(assetUpdateAvailable);
            }

            // Do Champion Statistics Update Process
            await DownloadChampionStatistics();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // Do Rollback
            SetUpdateStep(UpdateState.RollingBack);
            if (Directory.Exists("Backup"))
            {
                Directory.Delete("Cache", true);
                Directory.Move("Backup/Cache", "Cache");
                Directory.Delete("Backup", true);
            }

            var state = new ProgressState("Update Failed", 100);
            state.FinishState = ProgressFinishState.Failed;
            progress.Report(state);
        }
        
        // Clean Up
        SetUpdateStep(UpdateState.CleanUp);
        if (Directory.Exists("Backup"))
        {
            Directory.Delete("Backup", true);
        }
        if (Directory.Exists("Download"))
        {
            Directory.Delete("Download", true);
        }
        
        
        SetUpdateStep(UpdateState.ApplyingNewVersion);
        // Write new Version in Database
        ConfigData? version = _db.ConfigDatas.FirstOrDefault(x => x.ConfigName == "Version");
        if (version != null)
        {
            version.ConfigValue = assetUpdateAvailable?.ToString() ?? version.ConfigValue;
            _db.ConfigDatas.Update(version);
        }
        else
        {
            _db.ConfigDatas.Add(new ConfigData {ConfigName = "Version", ConfigValue = assetUpdateAvailable?.ToString() ?? "0.0.0"});
        }
        
        // Write Last Update Time in Database
        ConfigData? lastUpdate = _db.ConfigDatas.FirstOrDefault(x => x.ConfigName == "LastUpdate");
        if (lastUpdate != null)
        {
            lastUpdate.ConfigValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _db.ConfigDatas.Update(lastUpdate);
        }
        else
        {
            _db.ConfigDatas.Add(new ConfigData {ConfigName = "LastUpdate", ConfigValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")});
        }
        
        _db.SaveChanges();
        
        // Console.WriteLine("test");
        ChampionViewModel._championService.UpdateChampionDataFromCache();
        
        SetUpdateStep(UpdateState.Finished);
    }

    private async Task DownloadChampionStatistics()
    {
        SetUpdateStep(UpdateState.ReceivingChampionStatistics);
        
        // Store API Call to https://cdn.merakianalytics.com/riot/lol/resources/latest/en-US/championrates.json as Json 
        HttpClient client = new HttpClient();
        string response = await client.GetStringAsync("https://cdn.merakianalytics.com/riot/lol/resources/latest/en-US/championrates.json");
        File.WriteAllText("Cache/championRates.json", response);
    }

    private async Task StartAssetUpdate(Version newVersion)
    {
        SetUpdateStep(UpdateState.BackupOldAssets);
        // Backup all old assets
        if (Directory.Exists("Backup"))
        {
            Directory.Delete("Backup", true);
        }
        Directory.CreateDirectory("Backup");
        
        if (Directory.Exists("Cache"))
        {
            Directory.Move("Cache", "Backup/Cache");
        }
        
        SetUpdateStep(UpdateState.DownloadingAssets); 
        await DownloadNewAssets(newVersion.ToString());
        SetUpdateStep(UpdateState.ExtractingAssetsStep1);
        
        // Extract all new assets
        Directory.CreateDirectory("Download/Extract");
        ExtractGZipToTar();
        
        SetUpdateStep(UpdateState.ExtractingAssetsStep2);
        ExtractTarFile(newVersion.ToString());
        
        SetUpdateStep(UpdateState.ApplyingAssets);
        // Apply all new assets
        Directory.CreateDirectory("Cache");
        Directory.CreateDirectory("Cache/riot");
        Directory.Move("Download/Extract", "Cache/riot/dragontail");
        Directory.Move($"Cache/riot/dragontail/{newVersion.ToString()}", "Cache/riot/dragontail/latest");
        
        // Clean up not needed directories
        SetUpdateStep(UpdateState.CleanUpDownload);
        Directory.Delete("Download", true);
    }

    private void ExtractTarFile(string versionName)
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = $"{currentDirectory}/Depedencies/7z.exe",
            Arguments = $"x -o{currentDirectory}/Download/Extract -y -bsp1 {currentDirectory}/Download/dragontail-{versionName}.tar", // 
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        Process process = new Process {StartInfo = startInfo};
        process.EnableRaisingEvents = true;
        process.OutputDataReceived += (sender, args) =>
        {
            if (args.Data != null)
            {
                // Console.WriteLine(args.Data);
                string pattern = @"(?'progress'\d{1,})%";
                foreach (Match m in Regex.Matches(args.Data, pattern))
                {
                    SetUpdateStep(UpdateState.ExtractingAssetsStep2, double.Parse(m.Groups["progress"].Value));
                }
            }
        };
        process.Start();
        process.BeginOutputReadLine();
        process.WaitForExit();
    }

    private void ExtractGZipToTar()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = $"{currentDirectory}/Depedencies/7z.exe",
            Arguments = $"x -o{currentDirectory}/Download -y -bsp1 {currentDirectory}/Download/dragontail.tgz", // 
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        Process process = new Process {StartInfo = startInfo};
        process.EnableRaisingEvents = true;
        process.OutputDataReceived += (sender, args) =>
        {
            if (args.Data != null)
            {
                // Console.WriteLine(args.Data);
                string pattern = @"(?'progress'\d{1,})%";
                foreach (Match m in Regex.Matches(args.Data, pattern))
                {
                    SetUpdateStep(UpdateState.ExtractingAssetsStep1, double.Parse(m.Groups["progress"].Value));
                }
            }
        };
        process.Start();
        process.BeginOutputReadLine();
        process.WaitForExit();
    }

    private async Task DownloadNewAssets(string newVersion)
    {
        // Download all new assets
        if (Directory.Exists("Download"))
        {
            Directory.Delete("Download", true);
        }
        Directory.CreateDirectory("Download");
        HttpClient client = new HttpClient();
        var fs = new FileStream("Download/dragontail.tgz", FileMode.Create, FileAccess.Write, FileShare.None);
        Progress<float> downloadProgress = new Progress<float>();
        downloadProgress.ProgressChanged += (sender, e) =>
        {
            double process = Math.Round((double)e * 100, 2);
            // Console.WriteLine(process);
            SetUpdateStep(UpdateState.DownloadingAssets, process);
        };
        await client.DownloadAsync($"https://ddragon.leagueoflegends.com/cdn/dragontail-{newVersion}.tgz", fs, downloadProgress);
        fs.Close();
        
    }
  
    private void SetUpdateStep(UpdateState state)
    {
        if (state == UpdateState.Finished)
        {
            progress.Report(new ProgressState(state.ToString(), 100));
            _currentState = state;
            return;
        }
        
        progress.Report(new ProgressState(Regex.Replace(state.ToString(), "([a-z])([A-Z0-9])", "$1 $2"), (float)state / _allSteps * 100));
        _currentState = state;
    }
    
    private void SetUpdateStep(UpdateState state, double currentSetProgress)
    {
        if (state == UpdateState.Finished)
        {
            progress.Report(new ProgressState(state.ToString(), 100));
            _currentState = state;
            return;
        }
        
        progress.Report(new ProgressState(Regex.Replace($"{state.ToString()} ({currentSetProgress}%)", "([a-z])([A-Z0-9])", "$1 $2"), (float)state / _allSteps * 100));
        _currentState = state;
    }
    
    private async Task<Version?> IsAssetUpdateAvailable()
    {
        // get current version from database
        ConfigData? version = _db.ConfigDatas.FirstOrDefault(x => x.ConfigName == "Version");
        Console.WriteLine("Current Database Version: " + version?.ConfigValue);
        // get latest version from riot via API Call to https://ddragon.leagueoflegends.com/api/versions.json
        
        HttpClient client = new HttpClient();

        // Call asynchronous network methods in a try/catch block to handle exceptions
        try 
        {
            string response = await client.GetStringAsync("https://ddragon.leagueoflegends.com/api/versions.json");
            List<string> versions = JsonSerializer.Deserialize<List<string>>(response)!;

            // Console.WriteLine(versions[0]);
            
            Version latestVersion = new(versions[0]);
            Version currentVersion = new(version?.ConfigValue ?? "0.0.0");
            
            if (latestVersion > currentVersion)
            {
                Console.WriteLine("Update Available!");
                return latestVersion;
            }
            else
            {
                Console.WriteLine("No Update Available!");
                return null;
            }
        }  
        catch(HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");    
            Console.WriteLine("Message :{0} ",e.Message);
        }
        
        // @TODO: Show Error Message in Dialog and stop Update Process
        throw new Exception("Could not check for Asset Update!");
    }
    
}