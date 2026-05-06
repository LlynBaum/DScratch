using DScratch.Client.JsBridge;
using DScratch.Client.JsBridge.ProseMirrorCommand;

namespace DScratch.Client.Components;

public partial class EditorMenu(IPmBridge pmBridge)
{
    private string color = "#000000";
    
    private async Task BoldAsync()
    {
        await pmBridge.DispatchCommandAsync(Commands.ToggleMark.Strong);
    }

    private async Task ItalicAsync()
    {
        await pmBridge.DispatchCommandAsync(Commands.ToggleMark.Italic);
    }
    
    private async Task CodeAsync()
    {
        await pmBridge.DispatchCommandAsync(Commands.ToggleMark.Code);
    }

    public async Task BlockquoteAsync() // TODO: add function to revert this
    {
        await pmBridge.DispatchCommandAsync(Commands.WrapIn.Blockquote);
    }
    
    private async Task ParagraphAsync()
    {
        await pmBridge.DispatchCommandAsync(Commands.SetBlockType.Paragraph);
    }
    
    private async Task HeadingAsync(ushort level)
    {
        await pmBridge.DispatchCommandAsync(Commands.SetBlockType.Heading(level));
    }
    
    private async Task CodeBlockAsync()
    {
        await pmBridge.DispatchCommandAsync(Commands.SetBlockType.CodeBlock);
    }
    
    private async Task OnColorChangeAsync()
    {
        var command = Commands.AddMark.Color(color);
        await pmBridge.DispatchCommandAsync(command);
    }
}