using DScratch.Client.Services;
using Microsoft.AspNetCore.Components;

namespace DScratch.Client.Pages.Editor.Components.Debugging;

public partial class DebugPanel(EditorDebugService debugService)
{
    [Parameter]
    public bool IsOpen { get; set; }

    private string currentTab = nameof(DebugTreePanel);

    private bool IsConsoleLogEnabled
    {
        get => debugService.IsDebugEnabled;
        set => debugService.IsDebugEnabled = value;
    }
    
    private void TogglePanel()
    {
        IsOpen = !IsOpen;
    }

    private void SwitchTab(string tab) => currentTab = tab;
}