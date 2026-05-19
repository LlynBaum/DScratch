using DScratch.Transactions;
using DScratch.Transactions.Steps;

namespace DScratch;

public class DScratchService : IDScratchService
{
    // TODO: history of past transaction, so things like ctrl-z can be possible.
    private readonly Stack<DTransaction> transactions = [];
    
    public TransactionResult Apply(DTransaction transaction)
    {
        transactions.Push(transaction);
        return transaction.Commit();
    }
}