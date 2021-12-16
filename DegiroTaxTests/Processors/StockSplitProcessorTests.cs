using Microsoft.VisualStudio.TestTools.UnitTesting;
using DegiroTax.Processors;
using System;
using System.Collections.Generic;
using System.Text;
using DegiroTax.Classes;

namespace DegiroTax.Processors.Tests
{
    [TestClass()]
    public class StockSplitProcessorTests
    {
        [TestMethod()]
        public void processTest_stockSplit_PriceDoubledQuantityHalved()
        {
            var stockA = new Stock("stock1","isin1");
            var transaction1 = new Transaction.Builder().Stock(stockA).Price(100).Quantity(10).TransactionFee(.11).Id("1").DateTime(DateTime.Parse("2/16/2020 12:15:12 PM")).Build();
            var transaction2 = new Transaction.Builder().Stock(stockA).Price(110).Quantity(-5).TransactionFee(.12).Id("2").DateTime(DateTime.Parse("1/10/2021 12:15:12 PM")).Build();
            var transaction3 = new Transaction.Builder().Stock(stockA).Price(120).Quantity(-5).TransactionFee(0).Id("").DateTime(DateTime.Parse("5/10/2021 12:15:12 PM")).Build();
            var transaction4 = new Transaction.Builder().Stock(stockA).Price(240).Quantity(2.5).TransactionFee(0).Id("").DateTime(DateTime.Parse("5/10/2021 12:15:12 PM")).Build();
            var transactions = new List<Transaction>() {transaction1, transaction2, transaction3, transaction4 };

            var processedTransactions = new StockSplitProcessor().Process(transactions);

            Assert.AreEqual(2, processedTransactions.Count);
            Assert.AreEqual(200, processedTransactions[0].Price);
            Assert.AreEqual(220, processedTransactions[1].Price);
            Assert.AreEqual(5, processedTransactions[0].Quantity);
            Assert.AreEqual(-2.5, processedTransactions[1].Quantity);
        }

        [TestMethod]
        public void processTest_stockReverseSplit_PriceHalvedQuantityDoubled()
        {
            var stockA = new Stock("stock1", "isin1");
            var transaction1 = new Transaction.Builder().Stock(stockA).Price(100).Quantity(10).TransactionFee(.11).Id("1").DateTime(DateTime.Parse("2/16/2020 12:15:12 PM")).Build();
            var transaction2 = new Transaction.Builder().Stock(stockA).Price(110).Quantity(-5).TransactionFee(.12).Id("2").DateTime(DateTime.Parse("1/10/2021 12:15:12 PM")).Build();
            var transaction3 = new Transaction.Builder().Stock(stockA).Price(120).Quantity(-5).TransactionFee(0).Id("").DateTime(DateTime.Parse("5/10/2021 12:15:12 PM")).Build();
            var transaction4 = new Transaction.Builder().Stock(stockA).Price(60).Quantity(10).TransactionFee(0).Id("").DateTime(DateTime.Parse("5/10/2021 12:15:12 PM")).Build();
            var transactions = new List<Transaction>() { transaction1, transaction2, transaction3, transaction4 };

            var processedTransactions = new StockSplitProcessor().Process(transactions);

            Assert.AreEqual(2, processedTransactions.Count);
            Assert.AreEqual(50, processedTransactions[0].Price);
            Assert.AreEqual(55, processedTransactions[1].Price);
            Assert.AreEqual(20, processedTransactions[0].Quantity);
            Assert.AreEqual(-10, processedTransactions[1].Quantity);
        }

    }
}


