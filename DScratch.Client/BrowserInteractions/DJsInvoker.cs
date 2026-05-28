using DScratch.Transactions;
using Microsoft.JSInterop;

namespace DScratch.Client.BrowserInteractions;

public sealed class DJsInvoker(IJSRuntime jsRuntime)
{
    public async Task InitializeEditor(DotNetObjectReference<InputEventHelper> dotNetRef)
    {
        await jsRuntime.InvokeVoidAsync(ScriptConstants.EditorInitialize, dotNetRef);
    }

    public async Task ApplyTransaction(TransactionResult transactionResult)
    {
        await jsRuntime.InvokeVoidAsync(ScriptConstants.ApplyTransactionJs, transactionResult);
    }
}