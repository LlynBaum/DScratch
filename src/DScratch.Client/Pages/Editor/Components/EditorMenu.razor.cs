using DScratch.Interactions.CommandHandlers;
using DScratch.Interactions.CommandHandlers.Commands;
using DScratch.Interactions.CommandHandlers.Handlers;
using DScratch.Interactions.UserStates;
using DScratch.Nodes.Marks;

namespace DScratch.Client.Pages.Editor.Components;

public partial class EditorMenu(IEditorCommandDispatcher dispatcher, IUserStateService userStateService) : IDisposable
{
    private const string DefaultTextColor = "#000000";

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
        await dispatcher.DispatchAsync(UpdateMarkCommand.Toggle(MarkKey.FontWeight, "bold"));
        viewModel.IsBoldActive = userStateService.CheckMark(MarkKey.FontWeight, out _);
    }

    private async Task ItalicAsync()
    {
        await dispatcher.DispatchAsync(UpdateMarkCommand.Toggle(MarkKey.FontStyle, "italic"));
        viewModel.IsItalicActive = userStateService.CheckMark(MarkKey.FontStyle, out _);
    }

    private async Task ParagraphAsync() => await dispatcher.DispatchAsync(new ChangeBlockTypeCommand(BlockNodeType.Paragraph));
    
    private async Task HeadingAsync(BlockNodeType blockNodeType) => await dispatcher.DispatchAsync(new ChangeBlockTypeCommand(blockNodeType));
    
    private async Task OnColorChangeAsync()
    {
        await dispatcher.DispatchAsync(UpdateMarkCommand.Add(MarkKey.Color, viewModel.ActiveColor ?? DefaultTextColor));
        viewModel.ActiveColor = userStateService.CheckMark(MarkKey.Color, out var color) ? color : DefaultTextColor;
    }

    private async Task ClearColorAsync()
    {
        await dispatcher.DispatchAsync(UpdateMarkCommand.Remove(MarkKey.Color));
        viewModel.ActiveColor = userStateService.CheckMark(MarkKey.Color, out var color) ? color : DefaultTextColor;
    }

    private void OnActiveMarksChanged()
    {
        viewModel = new ViewModel
        {
            IsBoldActive = userStateService.CheckMark(MarkKey.FontWeight, out _),
            IsItalicActive = userStateService.CheckMark(MarkKey.FontStyle, out _),
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