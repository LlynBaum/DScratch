using DScratch.Transactions;
using DScratch.Transactions.Steps;

namespace DScratch;

public class DScratchService : IDScratchService
{
    // TODO: history of past transaction, so things like ctrl-z can be possible.
    private readonly Stack<ITransaction> transactions = [];

    public ITransaction StartTransaction(DScratchDocument document)
    {
        return new DTransaction(document);
    }
    
    public TransactionResult Apply(ITransaction transaction)
    {
        transactions.Push(transaction);
        return transaction.Commit();
    }
}