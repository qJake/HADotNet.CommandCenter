using System.ComponentModel.DataAnnotations;

namespace HADotNet.CommandCenter.Models.Config.Themes
{
    public class ThemePage
    {
        [Display(Name = "Background Color")]
        public string BackgroundColor { get; set; }

        [Display(Name = "Background Image URL")]
        public string BackgroundImageUrl { get; set; }

        [Display(Name = "Background Image Blend Mode")]
        public string BackgroundImageBlendMode { get; set; }

        [Display(Name = "Background Gradient")]
        public string BackgroundGradient { get; set; }

        [Display(Name = "Font Name")]
        public string PageFontFace { get; set; }

        [Display(Name = "Font Weight")]
        public string PageFontWeight { get; set; }
    }
}
