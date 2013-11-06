using System;

namespace Webpay.Integration.CSharp.Util.Calculation
{
    public static class MathUtil
    {
        public static long ConvertFromDecimalToCentesimal(decimal value)
        {
            value = Math.Truncate(value * 10000) / 10000; //Remove insignificant decimals
            value = BankersRound(value); //Bankers rounding to two decimals
            value = value * 100; //Convert to centesimal value (kr -> öre, eur -> cent, and so on)
            return Convert.ToInt64(value); //Return as long
        }

        public static decimal BankersRound(decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.ToEven);
        }

        public static decimal ReverseVatRate(decimal rate)
        {
            return (1 - (1 / (1 + rate / 100)));
        }
    }
}
