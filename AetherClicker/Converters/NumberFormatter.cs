using System;
using System.Globalization;
using System.Windows.Data;

namespace ArcanaTradingCompany.Converters
{
    public class NumberFormatter : IValueConverter
    {
        private static readonly string[] Suffixes = { "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc" };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double number)
            {
                int suffixIndex = 0;
                while (number >= 1000 && suffixIndex < Suffixes.Length - 1)
                {
                    number /= 1000;
                    suffixIndex++;
                }

                return $"{number:N1}{Suffixes[suffixIndex]}";
            }
            return value?.ToString() ?? "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 