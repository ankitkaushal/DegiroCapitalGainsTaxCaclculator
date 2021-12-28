namespace DegiroTax.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DegiroTax.Classes;

    public class TaxCalculator : ITaxCalculator
    {
        public TaxCalculator(int taxYear)
        {
            this.TaxYear = taxYear;
        }

        public int TaxYear { get; }

        public List<ProfitAndLossTransaction> CalculateProfitAndLoss(List<Transaction> transactions)
        {
            var sellTransactions = new Dictionary<string, List<Transaction>>();
            var buyTransactions = new Dictionary<string, List<Transaction>>();
            var profitAndLossTransactions = new List<ProfitAndLossTransaction>();

            foreach (var transaction in transactions)
            {
                if (transaction.IsBuy())
                {
                    var buyTransactionsIsinKey = buyTransactions.GetValueOrDefault(transaction.Stock.Isin, new List<Transaction>());
                    buyTransactionsIsinKey.Add(transaction);
                    buyTransactions[transaction.Stock.Isin] = buyTransactionsIsinKey;
                }
                else
                {
                    var sellTransactionsIsinKey = sellTransactions.GetValueOrDefault(transaction.Stock.Isin, new List<Transaction>());
                    sellTransactionsIsinKey.Add(transaction);
                    sellTransactions[transaction.Stock.Isin] = sellTransactionsIsinKey;
                }
            }

            foreach (var isin in sellTransactions.Keys)
            {
                foreach (var sellTransaction in sellTransactions[isin])
                {
                    if (sellTransaction.DateTime.Year == this.TaxYear)
                    {
                        double quantityLeft = -sellTransaction.Quantity;
                        while (quantityLeft > 0)
                        {
                            for (var i = 0; i < buyTransactions[isin].Count && quantityLeft > 0; i++)
                            {
                                if (buyTransactions[isin][i] == null)
                                {
                                    continue;
                                }

                                if (buyTransactions[isin][i].Quantity > quantityLeft)
                                {
                                    var buyTransactionLeft = this.GetTransactionWithChangedQuantity(buyTransactions[isin][i], buyTransactions[isin][i].Quantity - quantityLeft);
                                    var buyTransactionDone = this.GetTransactionWithChangedQuantity(buyTransactions[isin][i], quantityLeft);
                                    profitAndLossTransactions.Add(new ProfitAndLossTransaction(buyTransactionDone, this.GetTransactionWithChangedQuantity(sellTransaction, -quantityLeft)));
                                    buyTransactions[isin][i] = buyTransactionLeft;
                                    quantityLeft = 0;
                                }
                                else
                                {
                                    profitAndLossTransactions.Add(new ProfitAndLossTransaction(buyTransactions[isin][i], this.GetTransactionWithChangedQuantity(sellTransaction, -buyTransactions[isin][i].Quantity)));
                                    quantityLeft -= buyTransactions[isin][i].Quantity;
                                    buyTransactions[isin][i] = null;
                                }
                            }
                        }
                    }
                }
            }

            return profitAndLossTransactions.OrderBy(t => t.BuyTransaction.DateTime).ToList();
        }

        public double CalculateTax(List<ProfitAndLossTransaction> profitAndLossTransactions)
        {
            var profit = 0.0;

            foreach (var pnlTransaction in profitAndLossTransactions)
            {
                profit += pnlTransaction.ActualProfit();
            }

            return profit;
        }

        private Transaction GetTransactionWithChangedQuantity(Transaction originalTransaction, double quantity)
        {
            var transactionFeePerUnit = originalTransaction.TransactionFee / originalTransaction.Quantity;
            return new Transaction.Builder().Stock(originalTransaction.Stock).Quantity(quantity).
                        TransactionFee(transactionFeePerUnit * quantity).Price(originalTransaction.Price).Id(originalTransaction.Id).DateTime(originalTransaction.DateTime).Build();
        }
    }
}
