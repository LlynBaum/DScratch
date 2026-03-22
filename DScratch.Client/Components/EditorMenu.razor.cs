using DScratch.Client.Updates;

namespace DScratch.Client.Components;

public partial class EditorMenu(UpdateDispatcher updateDispatcher)
{
    private async Task BoldAsync()
    {
        await updateDispatcher.DispatchAsync(Commands.ToggleMark.Strong);
    }
}