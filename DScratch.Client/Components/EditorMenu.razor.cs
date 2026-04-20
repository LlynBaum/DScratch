using DScratch.Client.JsBridge.ProseMirrorCommand;

namespace DScratch.Client.Components;

public partial class EditorMenu(UpdateDispatcher updateDispatcher)
{
    private async Task BoldAsync()
    {
        await updateDispatcher.DispatchAsync(Commands.ToggleMark.Strong);
    }

    private async Task ItalicAsync()
    {
        await updateDispatcher.DispatchAsync(Commands.ToggleMark.Italic);
    }
    
    private async Task CodeAsync()
    {
        await updateDispatcher.DispatchAsync(Commands.ToggleMark.Code);
    }

    public async Task BlockquoteAsync() // TODO: add function to revert this
    {
        await updateDispatcher.DispatchAsync(Commands.WrapIn.Blockquote);
    }
    
    private async Task ParagraphAsync()
    {
        await updateDispatcher.DispatchAsync(Commands.SetBlockType.Paragraph);
    }
    
    private async Task HeadingAsync(ushort level)
    {
        await updateDispatcher.DispatchAsync(Commands.SetBlockType.Heading(level));
    }
    
    private async Task CodeBlockAsync()
    {
        await updateDispatcher.DispatchAsync(Commands.SetBlockType.CodeBlock);
    }
}