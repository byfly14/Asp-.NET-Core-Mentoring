using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Asp_.NET_MVC_Core_Mentoring_Module1.Helpers
{
    [HtmlTargetElement("Download-Image")]
    public class DownloadImageTagHelper : TagHelper
    {
        public string ImageId { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);
            output.TagName = "a";
            output.Attributes.SetAttribute("href", $"/Categories/GetCategoryImage/{ImageId}");
            output.Content.SetContent("Download Category Image using Tag Helper");
        }
    }
}
