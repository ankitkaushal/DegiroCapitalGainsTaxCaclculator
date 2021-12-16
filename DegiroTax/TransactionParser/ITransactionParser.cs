using DegiroTax.Classes;
using System.Collections.Generic;

namespace DegiroTax.TransactionParser
{
    public interface ITransactionParser
    {
        IEnumerable<Transaction> Parse();
    }
}
