using DScratch.Nodes;

namespace DScratch;

internal class DNodeFactory(INodeIdGenerator nodeIdGenerator) : INodeFactory
{
    public TextNode String(string value, DNode? origin, DNode? rightOrigin)
    {
        var textNode = new TextNode(nodeIdGenerator.GetNextId(), origin, rightOrigin);
        
        var firstNode = new CharNode(
            value: value[0], 
            id: nodeIdGenerator.GetNextId(), 
            origin: null, 
            rightOrigin: null);
        textNode.AppendChild(firstNode);
        
        var current = firstNode;
        for (var i = 1; i < value.Length; i++)
        {
            var newNode = new CharNode(
                value: value[i],
                id: nodeIdGenerator.GetNextId(), 
                origin: current,
                rightOrigin: null);
            
            current.RightOrigin = newNode;
            current = newNode;
            textNode.AppendChild(newNode);
        }

        return textNode;
    }
    
    public CharNode Char(char value, CharNode? origin, CharNode? rightOrigin)
    {
        var id = nodeIdGenerator.GetNextId();
        return new CharNode(value, id, origin, rightOrigin);
    }
}