using DScratch.Interactions.CommandHandlers;
using DScratch.Interactions.CommandHandlers.Commands;
using DScratch.Interactions.UserStates;
using DScratch.Nodes.Marks;

namespace DScratch.Client.Pages.Editor.Components;

public partial class EditorMenu(IEditorCommandDispatcher dispatcher, IUserStateService userStateService) : IDisposable
{
    private ViewModel viewModel = new ViewModel
    {
        IsBoldActive = false,
        IsItalicActive = false,
        ActiveColor = "#000000"
    };

    protected override void OnInitialized()
    {
        userStateService.OnStateChange += OnActiveMarksChanged;
    }

    private async Task BoldAsync() => await dispatcher.UpdateMarkAsync(new Mark(MarkKey.Bold), UpdateMarkAction.Toggle);
    private async Task ItalicAsync() => await dispatcher.UpdateMarkAsync(new Mark(MarkKey.Italic), UpdateMarkAction.Toggle);
    private async Task ParagraphAsync() => await dispatcher.ChangeBlockTypeAsync(BlockNodeType.Paragraph);
    private async Task HeadingAsync(BlockNodeType blockNodeType) => await dispatcher.ChangeBlockTypeAsync(blockNodeType);
    private async Task OnColorChangeAsync() => await dispatcher.UpdateMarkAsync(new Mark(MarkKey.Color, viewModel.ActiveColor), UpdateMarkAction.Add);
    private async Task ClearColorAsync() => await dispatcher.UpdateMarkAsync(new Mark(MarkKey.Color), UpdateMarkAction.Remove);

    private void OnActiveMarksChanged()
    {
        viewModel = new ViewModel
        {
            IsBoldActive = userStateService.ActiveMarks.Contains(new Mark(MarkKey.Bold)),
            IsItalicActive = userStateService.ActiveMarks.Contains(new Mark(MarkKey.Italic)),
            ActiveColor = userStateService.ActiveMarks.FirstOrDefault(m => m.Key == MarkKey.Color).Value,
        };
    }

    public void Dispose()
    {
        userStateService.OnStateChange -= OnActiveMarksChanged;
    }
    
    private class ViewModel
    {
        public required bool IsBoldActive { get; init; }

        public required bool IsItalicActive { get; init; }

        public required string? ActiveColor { get; set; }
    }
}