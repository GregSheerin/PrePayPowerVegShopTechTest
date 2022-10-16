using System;
using System.Collections.Generic;
using System.Text;

namespace Prepay.VegetableShop.ConsoleWorkBench.EnumExtensions
{
    public static class EnumExtensions
    {
        public static string ConvertToString(this Enum enumToConvert)
        {
            return Enum.GetName(enumToConvert.GetType(), enumToConvert);
        }

    }
}
