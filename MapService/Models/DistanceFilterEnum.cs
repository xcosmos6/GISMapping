
using System.ComponentModel;

namespace MapService.Models
{
    public enum DistanceFilterEnum
    {
        [Description("0")]
        Zero,
        [Description("1")]
        One,
        [Description("5")]
        Five,
        [Description("10")]
        Ten,
        [Description("50")]
        Fifity,
        [Description("100")]
        OneHundred,
        [Description("500")]
        FiveHundred,
        [Description("1000")]
        OneThousand,
        [Description("5000")]
        FiveThousand,
        [Description("10000")]
        TenThousand,
        [Description("\u221E")]
        Infinity

    }
}
