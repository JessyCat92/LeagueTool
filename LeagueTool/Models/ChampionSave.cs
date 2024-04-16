using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeagueTool.Models;

public class ChampionSave
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    [Column(Order = 0)]
    public int Id { get; set; }

    [Required] public string ChampionName { get; set; }

    [Required] public string Version { get; set; }

    [Required] public string ImageName { get; set; }

    [Required] public int ChampionKey { get; set; }

    public bool Played { get; set; } = false;

    public int Tries { get; set; } = 0;

    [Required] public List<Lane> Lanes { get; set; } = new();
}

public enum Lane
{
    Top,
    Jungle,
    Mid,
    Bot,
    Support
}