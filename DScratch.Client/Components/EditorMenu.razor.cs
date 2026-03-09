using DScratch.Client.Updates;
using Microsoft.AspNetCore.Components;

namespace DScratch.Client.Components;

public partial class EditorMenu(UpdateDispatcher updateDispatcher) : ComponentBase
{
    private async Task BoldAsync()
    {
        await updateDispatcher.DispatchAsync(new ToggleMark("strong"));
    }
}