using System;
using System.Globalization;
using System.Windows.Data;

namespace AetherClicker.Utils
{
    public class NumberFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double number)
            {
                if (number >= 1_000_000_000_000)
                {
                    return $"{number / 1_000_000_000_000:F2}T";
                }
                if (number >= 1_000_000_000)
                {
                    return $"{number / 1_000_000_000:F2}B";
                }
                if (number >= 1_000_000)
                {
                    return $"{number / 1_000_000:F2}M";
                }
                if (number >= 1_000)
                {
                    return $"{number / 1_000:F2}K";
                }
                return number.ToString("F2", culture);
            }
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                if (double.TryParse(stringValue, out double result))
                {
                    return result;
                }
            }
            return 0.0;
        }
    }
} 