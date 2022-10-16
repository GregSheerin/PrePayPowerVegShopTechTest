using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PrePay.VegetableShop.Data.CsvParser
{
    public interface ICsvParser<T> where T : class
    {
        //There are two cases were I want to parses csvs into two seperate classes
        //Used generics for better abstract(and for DI, can pull out a ICSV<TheParsersIWant> to get the relevant implenation)
        public Task<List<T>> ParseCsv(StreamReader productsCsv);
    }
}
