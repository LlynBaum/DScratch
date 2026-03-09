using Microsoft.JSInterop;

namespace DScratch.Client.Updates;

public class UpdateDispatcher(IJSRuntime jsRuntime)
{
    public async Task DispatchAsync(CommandUpdate update)
    {
        await jsRuntime.InvokeVoidAsync("applyCSharpUpdate", update);
    }
}