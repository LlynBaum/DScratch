using DScratch.Transactions;
using DScratch.Transactions.Steps;

namespace DScratch;

public interface IDScratchService
{
    INodeFactory NodeFactory { get; }
    
    ITransaction StartTransaction();
    
    TransactionResult Apply(ITransaction transaction);
    
    TransactionResult InitialTransaction();
}