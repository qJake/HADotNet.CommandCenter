using System.ComponentModel.DataAnnotations;

namespace HADotNet.CommandCenter.Models.Config
{
    public class SystemSettings
    {
        [Required]
        [Url(ErrorMessage = "Enter the full URL starting with http:// or https://.")]
        [Display(Name = "Home Assistant Base URL")]
        public string BaseUri { get; set; }

        /// <summary>
        /// Set automatically if running in Hass.io mode. Contains the discovered external URL of Home Assistant, for retrieving image assets (camera feeds, etc).
        /// </summary>
        public string ExternalBaseUri { get; set; }

        [Required]
        [MinLength(100, ErrorMessage = "This doesn't look like a long-lived access token.")]
        [MaxLength(200, ErrorMessage = "This doesn't look like a long-lived access token.")]
        [Display(Name = "Long-Lived Access Token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets whether or not this is a Hass.io addon environment. Set automatically when initialized from a Hass.io addon container.
        /// </summary>
        public bool IsHassIo { get; set; }
    }
}
