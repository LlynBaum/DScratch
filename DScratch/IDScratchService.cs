using DScratch.Transactions;
using DScratch.Transactions.Steps;

namespace DScratch;

public interface IDScratchService
{
    ITransaction StartTransaction(DScratchDocument document);
    
    TransactionResult Apply(ITransaction transaction);
}