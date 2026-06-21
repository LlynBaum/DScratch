using DScratch.Transactions;

namespace DScratch;

public interface IDScratchService
{
    DScratchDocument Document { get; }
    bool IsDebugEnabled { get; set; }
    event Action? DocumentChanged;
    event Action? DebugModeChanged;

    ITransaction StartTransaction();
    
    TransactionResult Apply(ITransaction transaction);
    
    TransactionResult InitialTransaction();
}