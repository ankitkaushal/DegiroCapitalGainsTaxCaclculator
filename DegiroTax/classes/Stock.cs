using System;
using System.Collections.Generic;
using System.Text;

namespace DegiroTax.Classes
{
    public class Stock
    {
        public Stock(string name, string isin)
        {
            this.Name = name;
            this.Isin = isin;
        }

        public string Name { get; }

        public string Isin { get; }
    }
}
