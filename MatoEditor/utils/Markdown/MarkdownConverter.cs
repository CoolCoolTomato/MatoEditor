namespace MatoEditor.utils.Markdown;

using Markdig;
using System.Text;

public class MarkdownConverter
{
    private static readonly string CustomCss = @"
        <style>
            body {
                font-family: Arial, sans-serif;
                line-height: 1.6;
                color: #333;
                max-width: 800px;
                margin: 0 auto;
                padding: 20px;
            }
            h1, h2, h3 {
                color: #2c3e50;
            }
            code {
                background-color: #f4f4f4;
                padding: 2px 4px;
                border-radius: 4px;
            }
            pre {
                background-color: #f4f4f4;
                padding: 10px;
                border-radius: 4px;
                overflow-x: auto;
            }
            blockquote {
                border-left: 4px solid #ccc;
                margin: 0;
                padding-left: 16px;
                color: #555;
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

    public static string ConvertMarkdownToHtml(string markdown)
    {
        // Configure Markdig
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        // Convert Markdown to HTML
        string htmlBody = Markdown.ToHtml(markdown, pipeline);

        // Combine custom CSS with HTML body
        var htmlBuilder = new StringBuilder();
        htmlBuilder.AppendLine("<!DOCTYPE html>");
        htmlBuilder.AppendLine("<html lang=\"en\">");
        htmlBuilder.AppendLine("<head>");
        htmlBuilder.AppendLine("<meta charset=\"UTF-8\">");
        htmlBuilder.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        htmlBuilder.AppendLine("<title>Converted Markdown</title>");
        htmlBuilder.AppendLine(CustomCss);
        htmlBuilder.AppendLine("</head>");
        htmlBuilder.AppendLine("<body>");
        htmlBuilder.AppendLine(htmlBody);
        htmlBuilder.AppendLine("</body>");
        htmlBuilder.AppendLine("</html>");

        return htmlBuilder.ToString();
    }
}