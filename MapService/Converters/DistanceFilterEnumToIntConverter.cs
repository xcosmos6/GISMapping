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
    public class DistanceFilterEnumToIntConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case DistanceFilterEnum.Zero:
                    return 0;
                case DistanceFilterEnum.One:
                    return 1;
                case DistanceFilterEnum.Five:
                    return 50;
                case DistanceFilterEnum.Ten:
                    return 10;
                case DistanceFilterEnum.Fifity:
                    return 50;
                case DistanceFilterEnum.OneHundred:
                    return 100;
                case DistanceFilterEnum.FiveHundred:
                    return 500;
                case DistanceFilterEnum.OneThousand:
                    return 1000;
                case DistanceFilterEnum.FiveThousand:
                    return 5000;
                case DistanceFilterEnum.TenThousand:
                    return 10000;
                case DistanceFilterEnum.Infinity:
                    return "\u221E";
                default:
                    return 0;
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case "0":
                    return DistanceFilterEnum.Zero;
                case "5":
                    return DistanceFilterEnum.Five;
                case "10":
                    return DistanceFilterEnum.Ten;
                case "50":
                    return DistanceFilterEnum.Fifity;
                case "100":
                    return DistanceFilterEnum.OneHundred;
                case "500":
                    return DistanceFilterEnum.FiveHundred;
                case "1000":
                    return DistanceFilterEnum.OneThousand;
                case "5000":
                    return DistanceFilterEnum.FiveThousand;
                case "10000":
                    return DistanceFilterEnum.TenThousand;
                case "\u221E":
                    return DistanceFilterEnum.Infinity;
                default:
                    return DistanceFilterEnum.Zero;
            }
        }
    }
}
