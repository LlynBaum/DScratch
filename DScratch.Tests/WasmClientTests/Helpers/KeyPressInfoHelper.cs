using DScratch.Client.BrowserInteractions;
using DScratch.Client.BrowserInteractions.EventHandlers;

namespace DScratch.Tests.WasmClientTests.Helpers;

public class KeyPressInfoHelper
{
    public static KeyPressInfo GetKeyPressInfoDirectionNone(NodePath path, int offset)
    {
        return new KeyPressInfo
        {
            Data = "abc",
            Path = path.Path.Reverse().ToArray(),
            InputType = InsertTextHandler.EventName,
            Selection = new KeyPressInfo.SelectionInfo
            {
                Direction = SelectionDirection.None,
                Offset = offset,
                End = [],
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
    
    public static KeyPressInfo GetKeyPressInfo(NodePath path, int offset, NodePath endPath, int endOffset)
    {
        var direction = offset < endOffset ? SelectionDirection.Forward : SelectionDirection.Backward;

        var start = direction is SelectionDirection.Forward ? path.Path.Reverse().ToArray() : endPath.Path.Reverse().ToArray();
        var end = direction is SelectionDirection.Forward ? endPath.Path.Reverse().ToArray() : path.Path.Reverse().ToArray();
        
        return new KeyPressInfo
        {
            Data = "xyz",
            Path = start,
            InputType = InsertTextHandler.EventName,
            Selection = new KeyPressInfo.SelectionInfo
            {
                Direction = direction,
                Offset = offset,
                End = end,
                EndOffset = endOffset
            }
        };
    }
}