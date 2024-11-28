using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Sample.AspNetCore.Taghelpers;

[HtmlTargetElement("AssemblyVersion", TagStructure = TagStructure.NormalOrSelfClosing)]
public class AssemblyVersionTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var versionInfo = FileVersionInfo.GetVersionInfo(executingAssembly.Location);
        var productVersion = versionInfo.ProductVersion;
        
        output.TagName = "";
        output.Content.Append(productVersion);
    }
}