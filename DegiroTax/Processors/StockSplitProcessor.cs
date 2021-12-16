using DegiroTax.Classes;
using System;
using System.Collections.Generic;

namespace DegiroTax.Processors
{
    public class StockSplitProcessor : ITransactionProcessor
    {
        public List<Transaction> Process(List<Transaction> transactions)
        {
            for (var i = 0; i < transactions.Count; i++)
            {
                if (transactions[i].Id.Length == 0)
                {
                    if (transactions[i + 1].Id.Length != 0)
                    {
                        throw new Exception("Not a valid transaction file");
                    }

                    this.ScaleTransactions(transactions, i);
                    i++;
                }
            }

            var trueTransactions = new List<Transaction>();

            foreach (var transaction in transactions)
            {
                if (transaction.Id.Length != 0)
                {
                    trueTransactions.Add(transaction);
                }
            }

            return trueTransactions;
        }

        private void ScaleTransactions(List<Transaction> transactions, int splitIndex)
        {
            var splitStock = transactions[splitIndex].Stock;
            var scale = this.FindScale(transactions[splitIndex], transactions[splitIndex + 1]);

            for (var i = 0; i < splitIndex; i++)
            {
                if (transactions[i].Stock.Isin == splitStock.Isin)
                {
                    transactions[i] = this.ScaleTransaction(transactions[i], scale);
                }
            }
        }

        private double FindScale(Transaction transaction1, Transaction transaction2)
        {
            var scale = Math.Max(transaction1.Quantity, transaction2.Quantity) / Math.Min(transaction1.Quantity, transaction2.Quantity);
            return -scale;
        }

        private Transaction ScaleTransaction(Transaction transaction, double scale)
        {
            var scaledTransaction = new Transaction.Builder().Stock(transaction.Stock).Quantity(transaction.Quantity * scale).
                        TransactionFee(transaction.TransactionFee).Price(transaction.Price / scale).Id(transaction.Id).DateTime(transaction.DateTime).Build();
            return scaledTransaction;
        }
    }
}
