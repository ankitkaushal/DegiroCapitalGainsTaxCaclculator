using System;
using System.Collections.Generic;
using System.Text;

namespace DegiroTax.Classes
{
    public class ProfitAndLossTransaction
    {
        public ProfitAndLossTransaction(Transaction buyTransaction, Transaction sellTransaction)
        {
            this.BuyTransaction = buyTransaction;
            this.SellTransaction = sellTransaction;
            this.Profit = (-sellTransaction.Quantity * sellTransaction.Price) - sellTransaction.TransactionFee - (buyTransaction.Quantity * buyTransaction.Price) - buyTransaction.TransactionFee;
        }

        public Transaction BuyTransaction { get; }

        public Transaction SellTransaction { get; }

        public double Profit { get; }

        public bool IsWashSale()
        {
            return this.SellTransaction.DateTime.Subtract(this.BuyTransaction.DateTime).Days < 28 && this.Profit < 0;
        }

        public virtual double ActualProfit()
        {
            return this.Profit;
        }
    }
}
