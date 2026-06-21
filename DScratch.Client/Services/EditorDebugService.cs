using DScratch.Transactions;

namespace DScratch.Client.Services;

public class EditorDebugService
{
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

    public event Action<TransactionResult>? DocumentChanged;
    public event Action? DebugModeChanged;

    public void NotifyDocumentChanged(TransactionResult transactionResult)
    {
        DocumentChanged?.Invoke(transactionResult);
    }
}
