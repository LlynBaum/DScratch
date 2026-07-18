using DScratch.Client.Services;
using DScratch.Interactions;
using DScratch.Interactions.UserStates;
using DScratch.Nodes.Marks;

namespace DScratch.Client.Pages.Editor.Components.Debugging;

public partial class DebugSelection(EditorDebugService editorDebugService, IUserStateService userStateService) : IDisposable
{
    private SelectionInfo? CurrentSelection => editorDebugService.CurrentClientSelection;
    private IReadOnlySet<Mark> ActiveMarks => userStateService.ActiveMarks;
    private IReadOnlySet<Mark> PendingMarks => userStateService.PendingMarks;

    protected override void OnInitialized()
    {
        editorDebugService.SelectionChanged += OnSelectionChanged;
        userStateService.OnStateChange += OnStateChange;
    }

    private void OnSelectionChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    private void OnStateChange()
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        editorDebugService.SelectionChanged -= OnSelectionChanged;
        userStateService.OnStateChange -= OnStateChange;
    }
}
