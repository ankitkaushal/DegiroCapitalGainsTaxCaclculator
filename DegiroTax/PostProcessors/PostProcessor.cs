using DegiroTax.Classes;
using System;
using System.Collections.Generic;

namespace DegiroTax.PostProcessors
{
    public class PostProcessor : IPostProcessor
    {
        public List<ProfitAndLossTransaction> Process(List<ProfitAndLossTransaction> pnlTransactions)
        {
            var pnlTransactionsWithOffset = new List<ProfitAndLossTransaction>();
            for (var i = 0; i < pnlTransactions.Count; i++)
            {
                var pnlTransaction = pnlTransactions[i];
                if (pnlTransaction.IsWashSale())
                {
                    var offset = this.CalculateOffset(pnlTransactions, i + 1, pnlTransaction);
                    var pnlTransactionWithOffset = new ProfitAndLossTransactionWithOffset(pnlTransaction, offset);
                    pnlTransactionsWithOffset.Add(pnlTransactionWithOffset);
                }
                else
                {
                    pnlTransactionsWithOffset.Add(pnlTransaction);
                }
            }

            return pnlTransactionsWithOffset;
        }

        private double CalculateOffset(IList<ProfitAndLossTransaction> pnlTransactions, int startIndex, ProfitAndLossTransaction washSale)
        {
            double loss = 0;
            for (var i = startIndex; i < pnlTransactions.Count && this.WithinInterval(washSale, pnlTransactions[i]); i++)
            {
                var offsetTransaction = pnlTransactions[i];
                if (offsetTransaction.Profit > 0 && offsetTransaction.BuyTransaction.Stock.Isin == washSale.BuyTransaction.Stock.Isin)
                {
                    loss += offsetTransaction.Profit;
                }
            }

            return Math.Min(loss, -washSale.Profit);
        }

        private bool WithinInterval(ProfitAndLossTransaction washSale, ProfitAndLossTransaction offsetTransaction)
        {
            return offsetTransaction.BuyTransaction.DateTime.Subtract(washSale.BuyTransaction.DateTime).Days < 28;
        }
    }
}
