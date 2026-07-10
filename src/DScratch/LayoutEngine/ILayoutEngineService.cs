using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.LayoutEngine;

public interface ILayoutEngineService
{
    void AddRoot(DNode documentRoot);
    Task RenderAsync(DScratchDocument document, TransactionResult transactionResult);
}