using DScratch.Client.Services;
using DScratch.Transactions;

namespace DScratch.Client.Pages.Editor.Components.Debugging;

public partial class DebugTransaction(EditorDebugService debugService) : IDisposable
{
    private TransactionResult? lastTransaction;

    protected override void OnInitialized()
    {
        debugService.DocumentChanged += OnDocumentChane;
    }

    private void OnDocumentChane(TransactionResult transactionResult)
    {
        InvokeAsync(() =>
        {
            lastTransaction = transactionResult;
            StateHasChanged();
        });
    }

    public void Dispose()
    {
        debugService.DocumentChanged -= OnDocumentChane;
    }
}