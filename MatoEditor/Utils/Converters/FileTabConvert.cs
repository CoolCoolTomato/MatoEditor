using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace MatoEditor.Utils.Converters;

public class FileTabConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values[0] is string oldContent && values[1] is string newContent)
        {
            return oldContent != newContent;
        }
        return AvaloniaProperty.UnsetValue;
    }
}
