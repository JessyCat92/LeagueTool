using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace LeagueTool.Services.Converter;

public class BoolToFloatConverter: IValueConverter
{
    public static readonly BoolToFloatConverter Instance = new();
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            if (parameter != null && parameter is float)
            {
                return b ? 1 : parameter;
            }
            
            return b ? 1 : 0.5;
        }

        return 0.5;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}