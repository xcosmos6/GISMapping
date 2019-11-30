using MapService.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MapService.Converters
{
    public class SignEnumToStrConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case CompareOperatorEnum.LessOrEqual:
                    return "\u2264";
                case CompareOperatorEnum.GreaterThan:
                    return ">";
                default:
                    return "\u2264";
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case "\u2264":
                    return CompareOperatorEnum.LessOrEqual;
                case ">":
                    return CompareOperatorEnum.GreaterThan;
                default:
                    return CompareOperatorEnum.LessOrEqual;
            }
        }
    }
}
