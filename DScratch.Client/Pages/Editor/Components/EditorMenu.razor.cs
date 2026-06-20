using DScratch.Interactions.CommandHandlers;

namespace DScratch.Client.Pages.Editor.Components;

public partial class EditorMenu(IEditorCommandDispatcher editorCommandDispatcher)
{
    private string color = "#000000";
    
    private async Task BoldAsync()
    {
        
    }

    private async Task ItalicAsync()
    {
        
    }
    
    private async Task CodeAsync()
    {
        
    }

    private async Task BlockquoteAsync() 
    {
        
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
        
    }
}