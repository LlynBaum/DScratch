using DScratch.Interactions;

namespace DScratch.Client.Services;

public class EditorDebugService : IEditorDebugService
{
    private readonly List<DebugTransactionInfo> transactions = [];
    public IReadOnlyList<DebugTransactionInfo> Transactions => transactions;
    
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

    public void NotifyDocumentChanged(DebugTransactionInfo debugTransactionResult)
    {
        transactions.Add(debugTransactionResult);
        DocumentChanged?.Invoke();
    }

    public void NotifySelectionChange(SelectionInfo? selectionInfo)
    {
        CurrentClientSelection = selectionInfo;
        SelectionChanged?.Invoke();
    }
}