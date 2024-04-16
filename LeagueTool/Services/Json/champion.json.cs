using System.Collections.Generic;

namespace LeagueTool.Services.Json;

public class champion_json
{
    public string type { get; set; }
    public string format { get; set; }
    public string version { get; set; }
    public Dictionary<string, ChampJsonData> data { get; set; }
}