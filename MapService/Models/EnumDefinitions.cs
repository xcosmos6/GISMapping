
using System.ComponentModel;

namespace MapService.Models
{
    public enum DistanceFilterEnum
    {
        [Description("0")]
        Zero = 0,
        [Description("5")]
        Five = 5,
        [Description("10")]
        Ten = 10,
        [Description("50")]
        Fifity = 50,
        [Description("100")]
        OneHundred = 100,
        [Description("500")]
        FiveHundred = 500,
        [Description("1000")]
        OneThousand = 1000,
        [Description("5000")]
        FiveThousand = 5000,
        [Description("10000")]
        TenThousand = 10000,
        [Description("50000")]
        FiftyThousand = 50000,
        [Description("100000")]
        OneHundredThousand = 100000
    }

    public enum CompareOperatorEnum
    {
        [Description("\u2264")]
        LessOrEqual,
        [Description(">")]
        GreaterThan
    }
}
