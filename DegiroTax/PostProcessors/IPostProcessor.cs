using DegiroTax.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DegiroTax.PostProcessors
{
    public interface IPostProcessor
    {
        List<ProfitAndLossTransaction> Process(List<ProfitAndLossTransaction> pnlTransactions);
    }
}
