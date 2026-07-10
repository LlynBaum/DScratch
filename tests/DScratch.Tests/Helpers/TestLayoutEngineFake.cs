using DScratch.LayoutEngine;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Tests.Helpers;

public class TestLayoutEngineFake : ILayoutEngineService
{
    public void AddRoot(DNode documentRoot)
    {
        
    }

    public Task RenderAsync(DScratchDocument document, TransactionResult transactionResult)
    {
        return Task.CompletedTask;
    }
}