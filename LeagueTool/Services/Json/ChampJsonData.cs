using System.Collections.Generic;

namespace LeagueTool.Services.Json;

public class ChampJsonData
{
    public string version { get; set; }
    public string id { get; set; }
    public string key { get; set; }
    public string name { get; set; }
    public string title { get; set; }
    public string blurb { get; set; }
    public Dictionary<string, int> info { get; set; }
    public Dictionary<string, object> image { get; set; }
    public List<string> tags { get; set; }
    public string partype { get; set; }
    public Dictionary<string, float> stats { get; set; }
}