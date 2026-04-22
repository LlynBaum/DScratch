using DScratch.Client.JsBridge.ProseMirrorCommand;
using Microsoft.JSInterop;

namespace DScratch.Client.JsBridge;

public class PmBridge(IJSRuntime jsRuntime) : IPmBridge
{
    public PmTransaction StartTransaction()
    {
        return new PmTransaction(jsRuntime);
    }
    
    public async Task DispatchCommandAsync(UpdateCommand update)
    {
        await jsRuntime.InvokeVoidAsync("applyCSharpUpdate", update);
    }
}