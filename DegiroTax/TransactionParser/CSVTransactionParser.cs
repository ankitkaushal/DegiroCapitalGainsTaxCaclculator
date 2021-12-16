using DegiroTax.Classes;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace DegiroTax.TransactionParser
{
    public class CSVTransactionParser : ITransactionParser
    {
        private readonly string path;

        public CSVTransactionParser(string path)
        {
            this.path = path;
        }

        private enum Fields
        {
            Date, Time, Product, ISIN, Reference, Venue, Quantity, Price, PriceCurr, LocalValue, LocalValueCurr,
            Value, ValueCurr, Exchange, TransactionCosts, TransactioCostsCurr, Total, TotalCurr, OrderID,
        }

        public IEnumerable<Transaction> Parse()
        {
            List<Transaction> transactions = new List<Transaction>();
            using (TextFieldParser parser = new TextFieldParser(this.path))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();

                    // Check if it is the title row
                    if (fields[(int)Fields.Date] == "Date")
                    {
                        continue;
                    }

                    var name = fields[(int)Fields.Product];
                    var isin = fields[(int)Fields.ISIN];
                    var stock = new Stock(name, isin);
                    var exchange = this.ParseDouble(fields[(int)Fields.Exchange], 1);
                    var quantity = this.ParseDouble(fields[(int)Fields.Quantity], 0);
                    var transactionFee = this.ParseDouble(fields[(int)Fields.TransactionCosts], 0);
                    var price = this.ParseDouble(fields[(int)Fields.Price], 0) / exchange;
                    var id = fields[(int)Fields.OrderID];
                    var date = fields[(int)Fields.Date].Replace("/", "-");
                    var time = fields[(int)Fields.Time];

                    var dateTime = DateTime.ParseExact($"{date} {time}", "d-M-yyyy HH:mm", CultureInfo.InvariantCulture);
                    var transaction = new Transaction.Builder().Stock(stock).Quantity(quantity).
                        TransactionFee(transactionFee).Price(price).Id(id).DateTime(dateTime).Build();

                    transactions.Add(transaction);
                }
            }

            return transactions;
        }

        private double ParseDouble(string value, double defaultValue)
        {
            if (value.Length == 0)
            {
                return defaultValue;
            }
            else
            {
                return double.Parse(value);
            }
        }
    }
}
