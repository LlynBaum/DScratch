using DScratch.Interactions;

namespace DScratch.Client.Services;

public class EditorDebugFake : IEditorDebugService
{
    public IReadOnlyList<DebugTransactionInfo> Transactions { get; } = [];
    public SelectionInfo? CurrentClientSelection => null;

    public bool IsDebugEnabled
    {
        get => false;
        set
        {
            _ = value;
            DebugModeChanged?.Invoke();
        }
    }

    public event Action? DocumentChanged;
    public event Action? SelectionChanged;
    public event Action? DebugModeChanged;

    public void NotifyDocumentChanged(DebugTransactionInfo debugTransactionResult)
    {
        DocumentChanged?.Invoke();
    }

    public void NotifySelectionChange(SelectionInfo? selectionInfo) 
    { 
        SelectionChanged?.Invoke();
    }
}