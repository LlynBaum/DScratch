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
        return new KeyPressInfo
        {
            Data = "xyz",
            Path = path.Path.Reverse().ToArray(),
            InputType = InsertTextHandler.EventName,
            Selection = new KeyPressInfo.SelectionInfo
            {
                Direction = direction,
                Offset = offset,
                End = [],
                EndOffset = endOffset
            }
        };
    }
}