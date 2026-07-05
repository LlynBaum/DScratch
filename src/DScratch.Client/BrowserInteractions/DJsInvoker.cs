using DScratch.Interactions;
using DScratch.Transactions;
using Microsoft.JSInterop;

namespace DScratch.Client.BrowserInteractions;

public sealed class DJsInvoker(IJSRuntime jsRuntime)
{
    public async Task InitializeEditor(DotNetObjectReference<InputEventHelper> dotNetRef)
    {
        await jsRuntime.InvokeVoidAsync(ScriptConstants.EditorInitialize, dotNetRef);
    }

    public async Task<SelectionInfo> GetSelectionAsync()
    {
        return await jsRuntime.InvokeAsync<SelectionInfo>(ScriptConstants.GetSelectionJs);
    }
}