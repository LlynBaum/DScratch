using DScratch.Transactions;

namespace DScratch;

public interface IDScratchService
{
    DScratchDocument Document { get; }

    ITransaction StartTransaction();
    
    TransactionResult Apply(ITransaction transaction);
    
    TransactionResult InitialTransaction();
}