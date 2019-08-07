using System.ComponentModel.DataAnnotations;

namespace HADotNet.CommandCenter.Models.Config.Tiles
{
    public class BaseTile
    {
        [Display(Name = "Tile Name")]
        [Required(ErrorMessage = "Give it a name!")]
        public string Name { get; set; }
    }
}
