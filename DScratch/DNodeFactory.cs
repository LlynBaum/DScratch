using DScratch.Nodes;

namespace DScratch;

public class DNodeFactory(INodeIdGenerator nodeIdGenerator)
{
    public TextNode String(string value)
    {
        var textNode = new TextNode(nodeIdGenerator.GetNextId(), null, null, null);
        
        var firstNode = new CharNode(
            value: value[0], 
            id: nodeIdGenerator.GetNextId(), 
            origin: null, 
            rightOrigin: null, 
            parent: textNode);
        
        var current = firstNode;
        for (var i = 1; i < value.Length; i++)
        {
            var newNode = new CharNode(
                value: value[i],
                id: nodeIdGenerator.GetNextId(), 
                origin: current,
                rightOrigin: null,
                parent: textNode);
            
            current.RightOrigin = newNode;
            current = newNode;
        }

        return textNode;
    }
    
    public CharNode Char(char value)
    {
        var id = nodeIdGenerator.GetNextId();
        return new CharNode(value, id, null, null, null);
    }
}