using DScratch.Transactions;

namespace DScratch.Client.Services;

public class EditorDebugService
{
    private readonly List<TransactionResult> transactions = [];
    public IReadOnlyList<TransactionResult> Transactions => transactions;
    
    public bool IsDebugEnabled
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                DebugModeChanged?.Invoke();
            }
        }
    }
    
    public event Action? DocumentChanged;
    public event Action? DebugModeChanged;

    public void NotifyDocumentChanged(TransactionResult transactionResult)
    {
        transactions.Add(transactionResult);
        DocumentChanged?.Invoke();
    }
}
