using DScratch.Client.Updates;
using Microsoft.AspNetCore.Components;

namespace DScratch.Client.Pages;

public partial class EditorMenu(UpdateDispatcher updateDispatcher)
{
    private async Task BoldAsync()
    {
        await updateDispatcher.DispatchAsync(new ToggleMark("strong"));
    }
}