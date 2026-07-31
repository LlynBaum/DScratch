using DScratch.Client.Services;
using DScratch.Interactions;
using DScratch.Interactions.UserStates;
using DScratch.Marks;

namespace DScratch.Client.Pages.Editor.Components.Debugging;

public partial class DebugSelection(IEditorDebugService editorDebugService, IUserStateService userStateService) : IDisposable
{
    private SelectionInfo? CurrentSelection => editorDebugService.CurrentClientSelection;
    private IReadOnlyDictionary<MarkKey, string> ActiveMarks => userStateService.ActiveMarks;
    private IReadOnlyDictionary<MarkKey, string> PendingMarks => userStateService.PendingMarks;
    private IReadOnlySet<MarkKey> PendingMarkRemovals => userStateService.PendingMarkRemovals;

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
