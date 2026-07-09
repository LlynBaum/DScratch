using DScratch.LayoutEngine;
using DScratch.Transactions;

namespace DScratch.Tests.Helpers;

public class TestLayoutEngineFake : ILayoutEngineService
{
    public TransactionResult? TransactionResult { get; private set; }
    
    public Task RenderAsync(TransactionResult transactionResult)
    {
        TransactionResult = transactionResult;
        return Task.CompletedTask;
    }
}