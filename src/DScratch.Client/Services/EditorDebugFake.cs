using DScratch.Interactions;

namespace DScratch.Client.Services;

public class EditorDebugFake : IEditorDebugService
{
    public IReadOnlyList<DebugTransactionInfo> Transactions { get; } = [];
    public SelectionInfo? CurrentClientSelection => null;

    public bool IsDebugEnabled
    {
        get => false;
        set { }
    }

    public event Action? DocumentChanged;
    public event Action? SelectionChanged;
    public event Action? DebugModeChanged;
    
    public void NotifyDocumentChanged(DebugTransactionInfo debugTransactionResult) { }

    public void NotifySelectionChange(SelectionInfo selectionInfo) { }
}