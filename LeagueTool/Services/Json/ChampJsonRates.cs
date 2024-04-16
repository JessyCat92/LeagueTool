namespace LeagueTool.Services.Json;

public class ChampJsonRates
{
    public PlayRate TOP { get; set; }
    public PlayRate JUNGLE { get; set; }
    public PlayRate MIDDLE { get; set; }
    public PlayRate BOTTOM { get; set; }
    public PlayRate UTILITY { get; set; }
}

public class PlayRate
{
    public float playRate { get; set; }
}