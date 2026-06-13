using DScratch.Client.BrowserInteractions;
using DScratch.Client.BrowserInteractions.EventHandlers;
using DScratch.Client.BrowserInteractions.EventHandlers.Events;

namespace DScratch.Tests.WasmClientTests.Helpers;

public class KeyPressInfoHelper
{
    public static KeyPressInfo GetKeyPressInfoDirectionNone(NodeId nodeId, int anchorOffset)
    {
        return new KeyPressInfo
        {
            Data = "abc",
            InputType = InsertTextHandler.EventName,
            Selection = new SelectionInfo
            {
                Direction = SelectionDirection.None,
                AnchorId = nodeId.Value,
                AnchorOffset = anchorOffset,
                FocusId = nodeId.Value,
                FocusOffset = 0
            }
        };
    }
    
    public static KeyPressInfo GetKeyPressInfo(NodeId nodeId, int anchorOffset, int focusOffset)
    {
        var direction = anchorOffset < focusOffset ? SelectionDirection.Forward : SelectionDirection.Backward;
        
        return new KeyPressInfo
        {
            Data = "xyz",
            InputType = InsertTextHandler.EventName,
            Selection = new SelectionInfo
            {
                Direction = direction,
                AnchorId = nodeId.Value,
                AnchorOffset = anchorOffset,
                FocusId = nodeId.Value,
                FocusOffset = focusOffset
            }
        };
    }
    
    public static KeyPressInfo GetKeyPressInfo(
        NodeId anchorId, 
        int anchorOffset, 
        NodeId focusId, 
        int focusOffset, 
        SelectionDirection direction = SelectionDirection.Forward)
    {
        
        return new KeyPressInfo
        {
            Data = "xyz",
            InputType = InsertTextHandler.EventName,
            Selection = new SelectionInfo
            {
                Direction = direction,
                AnchorId = anchorId.Value,
                AnchorOffset = anchorOffset,
                FocusId = focusId.Value,
                FocusOffset = focusOffset
            }
        };
    }
}