using DScratch.Interactions;
using DScratch.Transactions;
using Microsoft.JSInterop;

namespace DScratch.Client.BrowserInteractions;

public sealed class DJsInvoker(IJSRuntime jsRuntime)
{
    public async Task InitializeEditor(DotNetObjectReference<BrowserEventHelper> dotNetRef)
    {
        await jsRuntime.InvokeVoidAsync(ScriptConstants.EditorInitialize, dotNetRef);
    }

    public async Task ApplyTransaction(TransactionResult transactionResult)
    {
        await jsRuntime.InvokeVoidAsync(ScriptConstants.ApplyTransactionJs, transactionResult);
    }

    public async Task<SelectionInfo?> GetEditorSelectionAsync()
    {
        return await jsRuntime.InvokeAsync<SelectionInfo>(ScriptConstants.GetEditorSelectionJs);
    }

    public async Task<Dictionary<string, int>> GetPageNumbers(IReadOnlyList<string> nodeIds)
    {
        var result = await jsRuntime.InvokeAsync<IReadOnlyList<(string nodeId, int pageNumber)>>(ScriptConstants.GetPageNumbersJs, nodeIds);
        return result.ToDictionary(r => r.nodeId, r => r.pageNumber);
    }
}