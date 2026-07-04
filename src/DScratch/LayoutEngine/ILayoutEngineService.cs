using DScratch.Transactions;

namespace DScratch.LayoutEngine;

public interface ILayoutEngineService
{
    Task LayoutAsync(TransactionResult transactionResult);
}