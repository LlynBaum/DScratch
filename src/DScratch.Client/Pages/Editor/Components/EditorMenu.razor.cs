using DScratch.Interactions.CommandHandlers;
using DScratch.Interactions.CommandHandlers.Commands;
using DScratch.Interactions.UserStates;
using DScratch.Nodes.Marks;

namespace DScratch.Client.Pages.Editor.Components;

public partial class EditorMenu(IEditorCommandDispatcher dispatcher, IUserStateService userStateService) : IDisposable
{
    private const string? DefaultTextColor = "#000000";

    private ViewModel viewModel = new ViewModel
    {
        IsBoldActive = false,
        IsItalicActive = false,
        ActiveColor = DefaultTextColor
    };

    protected override void OnInitialized()
    {
        userStateService.OnStateChange += OnActiveMarksChanged;
    }

    private async Task BoldAsync()
    {
        await dispatcher.UpdateMarkAsync(new Mark(MarkKey.Bold), UpdateMarkAction.Toggle);
        viewModel.IsBoldActive = userStateService.CheckMark(MarkKey.Bold, out _);
    }

    private async Task ItalicAsync()
    {
        await dispatcher.UpdateMarkAsync(new Mark(MarkKey.Italic), UpdateMarkAction.Toggle);
        viewModel.IsItalicActive = userStateService.CheckMark(MarkKey.Italic, out _);
    }

    private async Task ParagraphAsync() => await dispatcher.ChangeBlockTypeAsync(BlockNodeType.Paragraph);
    private async Task HeadingAsync(BlockNodeType blockNodeType) => await dispatcher.ChangeBlockTypeAsync(blockNodeType);
    private async Task OnColorChangeAsync()
    {
        await dispatcher.UpdateMarkAsync(new Mark(MarkKey.Color, viewModel.ActiveColor), UpdateMarkAction.Add);
        viewModel.ActiveColor = userStateService.CheckMark(MarkKey.Color, out var color) ? color : DefaultTextColor;
    }

    private async Task ClearColorAsync()
    {
        await dispatcher.UpdateMarkAsync(new Mark(MarkKey.Color), UpdateMarkAction.Remove);
        viewModel.ActiveColor = userStateService.CheckMark(MarkKey.Color, out var color) ? color : DefaultTextColor;
    }

    private void OnActiveMarksChanged()
    {
        viewModel = new ViewModel
        {
            IsBoldActive = userStateService.CheckMark(MarkKey.Bold, out _),
            IsItalicActive = userStateService.CheckMark(MarkKey.Italic, out _),
            ActiveColor = userStateService.CheckMark(MarkKey.Color, out var color) ? color : DefaultTextColor,
        };
        StateHasChanged();
    }

    public void Dispose()
    {
        userStateService.OnStateChange -= OnActiveMarksChanged;
    }
    
    private class ViewModel
    {
        public required bool IsBoldActive { get; set; }

        public required bool IsItalicActive { get; set; }

        public required string? ActiveColor { get; set; }
    }
}