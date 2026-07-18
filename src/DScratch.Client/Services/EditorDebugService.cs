using DScratch.Interactions;
using DScratch.Transactions;

namespace DScratch.Client.Services;

public class EditorDebugService
{
    private readonly List<TransactionInfo> transactions = [];
    public IReadOnlyList<TransactionInfo> Transactions => transactions;
    
    public SelectionInfo? CurrentClientSelection { get; private set; }
    
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
    public event Action? SelectionChanged;
    public event Action? DebugModeChanged;

    public void NotifyDocumentChanged(TransactionInfo transactionResult)
    {
        transactions.Add(transactionResult);
        DocumentChanged?.Invoke();
    }

    public record TransactionInfo(TransactionResult Result, KeyPressInfo KeyPressInfo);

    public void NotifySelectionChange(SelectionInfo selectionInfo)
    {
        CurrentClientSelection = selectionInfo;
        SelectionChanged?.Invoke();
    }
}
