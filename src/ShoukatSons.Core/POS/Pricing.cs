// File: src/ShoukatSons.Core/POS/Pricing.cs
using System;

namespace ShoukatSons.Core.POS
{
    public static class Pricing
    {
        public static decimal ApplyDiscount(decimal baseAmount, PosDiscountType type, decimal value)
        {
            return type switch
            {
                PosDiscountType.None        => Round2(baseAmount),
                PosDiscountType.Percent     => Round2(Math.Max(0m, baseAmount - (baseAmount * (value / 100m)))),
                PosDiscountType.FixedAmount => Round2(Math.Max(0m, baseAmount - value)),
                _                           => Round2(baseAmount),
            };
        }

        public static decimal Round2(decimal v) =>
            Math.Round(v, 2, MidpointRounding.AwayFromZero);
    }
}