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

    private async Task BlockquoteAsync() 
    {
        // TODO: this can revert the wrapIn, but how can I find out, if it is already wraped or not...
        var transaction = pmBridge.StartTransaction();
        transaction.LiftToTarget();
        await transaction.DispatchAsync();
        
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
        var tr = pmBridge.StartTransaction();
        var selection = await tr.GetSelectionAsync();
        tr.AddMark(selection, PmSchema.Marks.CreateColor(color)); 
        await tr.DispatchAsync();
    }
}