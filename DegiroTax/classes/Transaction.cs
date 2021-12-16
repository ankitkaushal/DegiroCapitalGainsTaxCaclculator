using System;

namespace DegiroTax.Classes
{
    public class Transaction
    {
        private Transaction()
        {
        }

        public Stock Stock { get; private set; }

        public double Quantity { get; private set; }

        public double Price { get; private set; }

        public double TransactionFee { get; private set; }

        public string Id { get; private set; }

        public DateTime DateTime { get; private set; }

        public bool IsBuy()
        {
            return this.Quantity > 0;
        }

        public override string ToString()
        {
            return $"stock: {this.Stock} quantity: {this.Quantity} Price: {this.Price} Id: {this.Id} Date {this.DateTime}  ";
        }

        public class Builder
        {
            private Transaction transaction;

            public Builder()
            {
                this.transaction = new Transaction();
            }

            public Builder Stock(Stock stock)
            {
                this.transaction.Stock = stock;
                return this;
            }

            public Builder Quantity(double quantity)
            {
                this.transaction.Quantity = quantity;
                return this;
            }

            public Builder Price(double price)
            {
                this.transaction.Price = price;
                return this;
            }

            public Builder DateTime(DateTime dateTime)
            {
                this.transaction.DateTime = dateTime;
                return this;
            }

            public Builder TransactionFee(double transactionFee)
            {
                this.transaction.TransactionFee = transactionFee;
                return this;
            }

            public Builder Id(string id)
            {
                this.transaction.Id = id;
                return this;
            }

            public Transaction Build()
            {
                return this.transaction;
            }
        }
    }
}
