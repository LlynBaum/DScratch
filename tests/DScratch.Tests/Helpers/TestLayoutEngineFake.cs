using DScratch.LayoutEngine;
using DScratch.Transactions;

namespace DScratch.Tests.Helpers;

public class TestLayoutEngineFake : ILayoutEngineService
{
    public Task RenderAsync(TransactionResult transactionResult)
    {
        return Task.CompletedTask;
    }
}