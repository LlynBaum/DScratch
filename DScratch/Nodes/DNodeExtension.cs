using DScratch.Nodes.NodeTypes;

namespace DScratch.Nodes;

public static class DNodeExtension
{
    extension(ParagraphNode paragraphNode)
    {
        public int GetTextLength()
        {
            var walker = new TreeWalker<TextNode>(paragraphNode);
            var length = 0;
            
            var current = walker.NextNode();
            while (current is not null)
            {
                length += current.Length;
                current = walker.NextNode();
            }

            return length;
        }
    }
    
    extension(DNode node)
    {
        internal int FindAbsolutTextOffset(CharNode child)
        {
            var (result, offset) = FindAbsolutTextOffsetCore(node, child.Parent!.Id, child.IsDeleted);
            var relativeOffset = !child.IsDeleted 
                ? result?.IndexOf(child) 
                : result?.ChildNodes.ToList().FindIndex(n => n.Id == child.Id);
            
            if (relativeOffset is null or -1)
            {
                throw new InvalidOperationException("Can not find absolut offset of node. Probably the node is node a child of given parent.");
            }
            
            return offset + relativeOffset.Value;
        }
        
        internal int FindAbsolutTextOffset(TextNode child)
        {
            var (result, offset) = FindAbsolutTextOffsetCore(node, child.Id, child.IsDeleted);
            if (result is null)
            {
                throw new InvalidOperationException("Can not find absolut offset of node. Probably the node is node a child of given parent.");
            }
            
            return offset;
        }
        
        internal int FindAbsolutTextOffset<TNode>(DNode child) where TNode : IDNode
        {
            var walker = new TreeWalker<TextNode, TNode>(node, child.IsDeleted);

            var offset = 0;
            var (currentTextNode, _) = walker.NextNode();
            while (walker.Current is not null)
            {
                if (walker.Current!.Id == child.Id)
                {
                    break;
                }

                if (currentTextNode is not null && !currentTextNode.IsDeleted)
                {
                    offset += currentTextNode.Length;
                }

                (currentTextNode, _) = walker.NextNode();
            }

            if (walker.Current is null)
            {
                throw new InvalidOperationException("Can not find absolut offset of node. Probably the node is node a child of given parent.");
            }
            
            return offset;
        }
    }
    
    private static (DNode? node, int offset) FindAbsolutTextOffsetCore(DNode parent, NodeId id, bool includeDeleted)
    {
        var walker = new TreeWalker<TextNode>(parent, includeDeleted);
            
        var offset = 0;
        var current = walker.NextNode();
        while (current is not null)
        {
            if (current.Id == id)
            {
                break;
            }
                
            if (!current.IsDeleted)
            {
                offset += current.Length;
            }
                
            current = walker.NextNode();
        }

        return (current, offset);
    }
}