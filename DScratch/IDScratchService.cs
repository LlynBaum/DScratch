using DScratch.Transactions;

namespace DScratch;

public interface IDScratchService
{
    ITransaction StartTransaction();
    
    TransactionResult Apply(ITransaction transaction);
    
    TransactionResult InitialTransaction();
}