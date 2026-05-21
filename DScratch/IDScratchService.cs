using DScratch.Transactions;
using DScratch.Transactions.Steps;

namespace DScratch;

public interface IDScratchService
{
    INodeFactory NodeFactory { get; }
    
    ITransaction StartTransaction(DScratchDocument document);
    
    TransactionResult Apply(ITransaction transaction);
}