using DScratch.Client.BrowserInteractions.EventHandlers.Common;
using DScratch.Client.BrowserInteractions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Client.BrowserInteractions.EventHandlers;

public class DeleteWordForwardHandler(IDScratchService dScratchService) : IEditorEventHandler
{
    public const string EventName = "deleteWordForward";
    
    public TransactionResult Handle(KeyPressInfo keyPressInfo)
    {
        var transaction = dScratchService.StartTransaction();
        
        var parent = transaction.FindNode(keyPressInfo.GetNodePath());
        if (parent is null)
        {
            throw new ArgumentException($"Parent with given path not found: {keyPressInfo.GetNodePath()}");
        }

        if (keyPressInfo.Selection.Direction is SelectionDirection.None)
        {
            var nodeInfo = SimpleDeleteBackwards(keyPressInfo, transaction, parent);
            if(nodeInfo.HasFoundNode) transaction.AddCursorPosition(nodeInfo.Node!.Id, nodeInfo.AbsolutOffset);
        }
        else
        {   
            var nodeSearchResult = DeleteSelection.Handle(keyPressInfo, transaction, parent);
            var cursorPosition = nodeSearchResult.Origin.AbsoluteOffsetIfPresent;
            var cursorTarget = nodeSearchResult.Origin.Node?.ParentElement ?? parent;
            if(cursorPosition is not null) transaction.AddCursorPosition(cursorTarget.Id, cursorPosition.Value);
        }

        return dScratchService.Apply(transaction);
    }
    
    private static NodeInfo SimpleDeleteBackwards(KeyPressInfo keyPressInfo, ITransaction transaction, DNode parent)
    {
        // TODO: search for the given node at the offset. Then go back and start delete everything until hitting a white space character.
        return null!;
    }
}