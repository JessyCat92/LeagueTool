using System.Collections.Generic;

namespace LeagueTool.Services.Json;

public class rates_json
{
    public string version { get; set; } = "";
    public Dictionary<int, ChampJsonRates> data { get; set; } = new();
}