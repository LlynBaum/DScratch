using Microsoft.JSInterop;

namespace DScratch.Client.Updates.ProseMirrorCommand;

public class UpdateDispatcher(IJSRuntime jsRuntime)
{
    public async Task DispatchAsync(UpdateCommand update)
    {
        await jsRuntime.InvokeVoidAsync("applyCSharpUpdate", update);
    }
}