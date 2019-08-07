using System.ComponentModel.DataAnnotations;

namespace HADotNet.CommandCenter.Models.Config
{
    public class SystemSettings
    {
        [Required]
        [Url(ErrorMessage = "Enter the full URL starting with http:// or https://.")]
        [Display(Name = "Home Assistant Base URL")]
        public string BaseUri { get; set; }

        [Required]
        [MinLength(100, ErrorMessage = "This doesn't look like a long-lived access token.")]
        [MaxLength(200, ErrorMessage = "This doesn't look like a long-lived access token.")]
        [Display(Name = "Long-Lived Access Token")]
        public string AccessToken { get; set; }
    }
}
