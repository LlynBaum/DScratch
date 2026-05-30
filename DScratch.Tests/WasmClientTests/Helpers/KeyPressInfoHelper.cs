using DScratch.Client.BrowserInteractions;
using DScratch.Client.BrowserInteractions.EventHandlers;

namespace DScratch.Tests.WasmClientTests.Helpers;

public class KeyPressInfoHelper
{
    public static KeyPressInfo GetKeyPressInfoDirectionNone(NodePath path, int offset)
    {
        var nodePath = path.Path.Reverse().ToArray();
        return new KeyPressInfo
        {
            Data = "abc",
            Path = nodePath,
            InputType = InsertTextHandler.EventName,
            Selection = new KeyPressInfo.SelectionInfo
            {
                Direction = SelectionDirection.None,
                Offset = offset,
                End = nodePath,
                EndOffset = 0
            }
        };
    }
    
    public static KeyPressInfo GetKeyPressInfo(NodePath path, int offset, int endOffset)
    {
        var direction = offset < endOffset ? SelectionDirection.Forward : SelectionDirection.Backward;
        var nodePath = path.Path.Reverse().ToArray();
        
        return new KeyPressInfo
        {
            Data = "xyz",
            Path = nodePath,
            InputType = InsertTextHandler.EventName,
            Selection = new KeyPressInfo.SelectionInfo
            {
                Direction = direction,
                Offset = offset,
                End = nodePath,
                EndOffset = endOffset
            }
        };
    }
    
    public static KeyPressInfo GetKeyPressInfo(
        NodePath path, 
        int offset, 
        NodePath endPath, 
        int endOffset, 
        SelectionDirection direction = SelectionDirection.Forward)
    {
        
        return new KeyPressInfo
        {
            Data = "xyz",
            Path = path.Path.Reverse().ToArray(),
            InputType = InsertTextHandler.EventName,
            Selection = new KeyPressInfo.SelectionInfo
            {
                Direction = direction,
                Offset = offset,
                End = endPath.Path.Reverse().ToArray(),
                EndOffset = endOffset
            }
        };
    }
}