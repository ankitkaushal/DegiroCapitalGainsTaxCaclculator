using DegiroTax.Classes;
using System.Collections.Generic;

namespace DegiroTax.Services
{
    public interface ITaxCalculator
    {
        List<ProfitAndLossTransaction> CalculateProfitAndLoss(List<Transaction> transactions);

        double CalculateTax(List<ProfitAndLossTransaction> profitAndLossTransactions);
    }
}
