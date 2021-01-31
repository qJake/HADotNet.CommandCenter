using HADotNet.CommandCenter.Utils;
using System.ComponentModel.DataAnnotations;

namespace HADotNet.CommandCenter.Models.Config
{
    public class SystemSettings
    {
        [Required]
        [Url(ErrorMessage = "Enter your Home Assistant URL starting with http:// or https://.")]
        [Display(Name = "Home Assistant Base URL")]
        public string BaseUri { get; set; }

        /// <summary>
        /// If set, overrides the base URL for assets (images, video feeds, etc) on the frontend.
        /// </summary>
        [Url(ErrorMessage = "Enter a URL beginning with http:// or https://")]
        [Display(Name = "Override Asset Base URL")]
        public string OverrideAssetUri { get; set; }

        [Required]
        [MinLength(100, ErrorMessage = "This doesn't look like a long-lived access token.")]
        [MaxLength(200, ErrorMessage = "This doesn't look like a long-lived access token.")]
        [Display(Name = "Long-Lived Access Token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets the Websocket URL.
        /// </summary>
        public string WebsocketUri => ((!string.IsNullOrWhiteSpace(OverrideAssetUri) ? OverrideAssetUri : BaseUri) ?? "http://undefined").TrimEnd('/').ToLower().Replace("https://", "wss://").Replace("http://", "ws://") + "/api/websocket";
    }
}
