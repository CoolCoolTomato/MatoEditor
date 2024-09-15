using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Platform;

namespace MatoEditor.utils.Converters;

public class SvgConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string stringValue)
        {
            var uri = stringValue;

            var stream = AssetLoader.Open(new Uri(uri));
            using (var reader = new StreamReader(stream))
            {
                var svgContent = reader.ReadToEnd();
                return StreamGeometry.Parse(ExtractSvgPath(svgContent));
            }
        }
        return AvaloniaProperty.UnsetValue; 
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return AvaloniaProperty.UnsetValue;
    }
    
    private static string ExtractSvgPath(string svgContent)
    {
        string pattern = @"<path[^>]*\s+d=""([^""]+)""[^>]*>";
        Regex regex = new Regex(pattern);
        Match match = regex.Match(svgContent);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }
        return string.Empty;
    }
}