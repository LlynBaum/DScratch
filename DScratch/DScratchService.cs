using DScratch.Transactions;
using DScratch.Transactions.Steps;

namespace DScratch;

public class DScratchService : IDScratchService
{
    private readonly Stack<DTransaction> transactions = [];
    
    public TransactionResult Apply(DTransaction transaction)
    {
        transactions.Push(transaction);
        return new TransactionResult(transaction.Commit());
    }
}