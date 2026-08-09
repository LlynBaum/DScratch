using DScratch.Interactions;
using DScratch.Transactions;

namespace DScratch.Client.Services;

public record DebugTransactionInfo(TransactionResult Result, KeyPressInfo KeyPressInfo);

public interface IEditorDebugService
{
    IReadOnlyList<DebugTransactionInfo> Transactions { get; }
    
    SelectionInfo? CurrentClientSelection { get; }
    
    bool IsDebugEnabled { get; set; }
    
    event Action? DocumentChanged;
    event Action? SelectionChanged;
    event Action? DebugModeChanged;

    void NotifyDocumentChanged(DebugTransactionInfo debugTransactionResult);

    void NotifySelectionChange(SelectionInfo? selectionInfo);
}