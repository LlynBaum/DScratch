using DScratch.Transactions;
using DScratch.Transactions.Steps;

namespace DScratch;

public interface IDScratchService
{
    TransactionResult Apply(DTransaction transaction);
}