using DScratch.Transactions;

namespace DScratch.LayoutEngine;

internal interface ILayoutEngineService
{
    Task LayoutAsync(TransactionResult transactionResult);
}