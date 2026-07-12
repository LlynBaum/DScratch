using DScratch.Interactions.CommandHandlers;
using DScratch.Interactions.CommandHandlers.Commands;
using DScratch.Nodes.Marks;

namespace DScratch.Client.Pages.Editor.Components;

public partial class EditorMenu(IEditorCommandDispatcher editorCommandDispatcher)
{
    private string color = "#000000";
    
    private async Task BoldAsync()
    {
        await editorCommandDispatcher.UpdateMarkAsync(new Mark(MarkKey.Bold), UpdateMarkAction.Toggle);
    }

    private async Task ItalicAsync()
    {
        await editorCommandDispatcher.UpdateMarkAsync(new Mark(MarkKey.Italic), UpdateMarkAction.Toggle);
    }
    
    private async Task ParagraphAsync()
    {
        await editorCommandDispatcher.ChangeBlockTypeAsync(BlockNodeType.Paragraph);
    }
    
    private async Task HeadingAsync(BlockNodeType blockNodeType)
    {
        await editorCommandDispatcher.ChangeBlockTypeAsync(blockNodeType);
    }
    
    private async Task CodeBlockAsync()
    {
        
    }

    private async Task OnColorChangeAsync()
    {
        await editorCommandDispatcher.UpdateMarkAsync(new Mark(MarkKey.Color, color), UpdateMarkAction.Add);
    }

    private async Task ClearColorAsync()
    {
        await editorCommandDispatcher.UpdateMarkAsync(new Mark(MarkKey.Color), UpdateMarkAction.Remove);
    }
}