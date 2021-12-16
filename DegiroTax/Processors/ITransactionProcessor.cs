using DegiroTax.Classes;
using System.Collections.Generic;

namespace DegiroTax.Processors
{
    public interface ITransactionProcessor
    {
        List<Transaction> Process(List<Transaction> transactions);
    }
}
