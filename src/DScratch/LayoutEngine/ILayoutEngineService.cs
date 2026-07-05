using DScratch.Transactions;

namespace DScratch.LayoutEngine;

public interface ILayoutEngineService
{
    Task RenderAsync(TransactionResult transactionResult);
}