using System;
using System.Collections.Generic;
using System.Text;

namespace DegiroTax.Classes
{
    public class ProfitAndLossTransactionWithOffset : ProfitAndLossTransaction
    {
        public ProfitAndLossTransactionWithOffset(ProfitAndLossTransaction pnlTransaction, double offset)
            : base(pnlTransaction.BuyTransaction, pnlTransaction.SellTransaction)
        {
            this.Offset = offset;
        }

        public double Offset { get; }

        public override double ActualProfit()
        {
            return this.Offset == 0 ? 0 : -this.Offset;
        }
    }
}
