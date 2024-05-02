using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LeagueTool.Models;

[Index(nameof(ConfigName), IsUnique = true)]
public class ConfigData
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    [Column(Order = 0)]
    public int Id { get; set; }

    [Required] public string ConfigName { get; set; }
    
    [Required] public string ConfigValue { get; set; }
}