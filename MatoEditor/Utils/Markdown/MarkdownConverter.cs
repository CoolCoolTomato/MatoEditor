using Avalonia;
using System.Text;
using Avalonia.Styling;
using Markdig;

namespace MatoEditor.Utils.Markdown;

public class MarkdownConverter
{
    private static readonly string LightCss = @"
        <style>
            body {
                font-family: Arial, sans-serif;
                line-height: 1.6;
                color: #202020;
                max-width: 800px;
                margin: 0 auto;
                padding: 20px;
                background-color: #FFFFFF;
            }
            h1, h2, h3, h4, h5, h6 {
                color: #202020;
            }
            code {
                background-color: #F8F8F8;
                padding: 2px 4px;
                border-radius: 4px;
            }
            pre {
                background-color: #F8F8F8;
                padding: 10px;
                border-radius: 4px;
                overflow-x: auto;
            }
            blockquote {
                border-left: 4px solid #F8F8F8;
                margin: 0;
                padding-left: 16px;
                color: #202020;
            }
            img {
                max-width: 100%;
                height: auto;
            }
            a {
                color: #3498db;
                text-decoration: none;
            }
            a:hover {
                text-decoration: underline;
            }
        </style>";
    
    private static readonly string DarkCss = @"
        <style>
            body {
                font-family: Arial, sans-serif;
                line-height: 1.6;
                color: #F0F0F0;
                max-width: 800px;
                margin: 0 auto;
                padding: 20px;
                background-color: #101010;
            }
            h1, h2, h3, h4, h5, h6 {
                color: #F0F0F0;
            }
            code {
                background-color: #202020;
                padding: 2px 4px;
                border-radius: 4px;
            }
            pre {
                background-color: #202020;
                padding: 10px;
                border-radius: 4px;
                overflow-x: auto;
            }
            blockquote {
                border-left: 4px solid #ccc;
                margin: 0;
                padding-left: 16px;
                color: #F0F0F0;
            }
            img {
                max-width: 100%;
                height: auto;
            }
            a {
                color: #3498db;
                text-decoration: none;
            }
            a:hover {
                text-decoration: underline;
            }
        </style>";

    public static string ConvertMarkdownToHtml(string markdown, string theme)
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();
        string htmlBody = Markdig.Markdown.ToHtml(markdown, pipeline);

        var htmlBuilder = new StringBuilder();
        htmlBuilder.AppendLine("<!DOCTYPE html>");
        htmlBuilder.AppendLine("<html lang=\"en\">");
        htmlBuilder.AppendLine("<head>");
        htmlBuilder.AppendLine("<meta charset=\"UTF-8\">");
        htmlBuilder.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        htmlBuilder.AppendLine("<title>Converted Markdown</title>");
        if (theme == "Light")
        {
            htmlBuilder.AppendLine(LightCss);   
        }
        else
        {
            htmlBuilder.AppendLine(DarkCss);   
        }
        htmlBuilder.AppendLine("</head>");
        htmlBuilder.AppendLine("<body>");
        htmlBuilder.AppendLine(htmlBody);
        htmlBuilder.AppendLine("</body>");
        htmlBuilder.AppendLine("</html>");

        return htmlBuilder.ToString();
    }
}