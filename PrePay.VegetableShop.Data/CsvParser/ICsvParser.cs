﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PrePay.VegetableShop.Data.CsvParser
{
    public interface ICsvParser<T> where T : class
    {
        public Task<List<T>> ParseCsv(StreamReader productsCsv);
    }
}
