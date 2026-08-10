using DScratch.Interactions.CommandHandlers;
using DScratch.Interactions.CommandHandlers.Commands;
using DScratch.Interactions.UserStates;
using DScratch.Marks;
using Microsoft.AspNetCore.Components.Web;

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
        await dispatcher.DispatchAsync<IMarkCommand>(new ToggleFontWeight());
        viewModel.IsBoldActive = userStateService.CheckMark(MarkKey.FontWeight, out var fontWeight) && fontWeight is "bold";
        StateHasChanged();
    }

    private async Task ItalicAsync()
    {
        await dispatcher.DispatchAsync<IMarkCommand>(new ToggleFontStyle());
        viewModel.IsItalicActive = userStateService.CheckMark(MarkKey.FontStyle, out var fontStyle) && fontStyle is "italic";
        StateHasChanged();
    }
    
    private async Task OnColorChangeAsync()
    {
        await dispatcher.DispatchAsync<IMarkCommand>(new SetColor(viewModel.ActiveColor ?? DefaultTextColor));
        viewModel.ActiveColor = userStateService.CheckMark(MarkKey.Color, out var color) ? color : DefaultTextColor;
    }

    private async Task ClearColorAsync()
    {
        await dispatcher.DispatchAsync<IMarkCommand>(new SetColor(DefaultTextColor));
        viewModel.ActiveColor = userStateService.CheckMark(MarkKey.Color, out var color) ? color : DefaultTextColor;
    }

    private async Task ParagraphAsync() => await dispatcher.DispatchAsync(new ChangeBlockTypeCommand(BlockNodeType.Paragraph));
    
    private async Task HeadingAsync(BlockNodeType blockNodeType) => await dispatcher.DispatchAsync(new ChangeBlockTypeCommand(blockNodeType));
    
    private string linkUrl = string.Empty;

    private async Task SubmitLinkAsync()
    {
        if (!string.IsNullOrWhiteSpace(linkUrl))
        {
            await dispatcher.DispatchAsync(new AddLinkCommand(linkUrl, "_self"));
        }
    }

    private async Task HandleLinkKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await SubmitLinkAsync();
        }
    }

    private async Task RemoveLinkAsync()
    {
        await dispatcher.DispatchAsync(new RemoveLinkCommand());
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