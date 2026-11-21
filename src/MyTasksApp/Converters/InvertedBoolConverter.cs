using System.Globalization;
using Microsoft.Maui.Controls;

namespace MyTasksApp.Converters;

/// <summary>
/// Converter đảo ngược giá trị boolean (true -> false, false -> true)
/// </summary>
public class InvertedBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return !b;

        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return !b;

        return false;
    }
}
