using DScratch.Transactions;

namespace DScratch;

public interface IDScratchService
{
    DScratchDocument Document { get; }

    ITransaction StartTransaction();
    
    Task ApplyAsync(ITransaction transaction);
    
    Task InitialTransactionAsync();
}