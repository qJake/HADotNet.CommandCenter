using HADotNet.CommandCenter.Models.Config.Tiles;
using HADotNet.CommandCenter.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace HADotNet.CommandCenter.Utils
{
    public class UniqueNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var configService = (IConfigStore)validationContext.GetService(typeof(IConfigStore));

            var config = configService.GetConfigAsync().GetAwaiter().GetResult();

            var existingTiles = config.Pages
                .Where(p => p.Tiles.Any(t => t.Name.ToUpper() == value?.ToString().ToUpper()))
                .Select(p => p.Name);

            if (existingTiles.Any())
            {
                return new ValidationResult("Tile names must be unique. Name already exists on page: " + string.Join(", ", existingTiles.ToArray()));
            }

            return ValidationResult.Success;
        }
    }
}