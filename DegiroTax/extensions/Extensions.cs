using System;
using System.Collections.Generic;
using System.Text;

namespace DegiroTax.Extensions
{
    public static class Extensions
    {
        public static double RoundOff(this double value)
        {
            return Math.Round(value, 2);
        }
    }
}
