using DScratch.Transactions;

namespace DScratch.LayoutEngine;

public interface ILayoutEngineService
{
    Task RenderAsync(DScratchDocument document, TransactionResult transactionResult);
}