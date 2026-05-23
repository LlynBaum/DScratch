using DScratch.Nodes.NodeTypes;

namespace DScratch.Nodes;

public static class DNodeExtension
{
    extension(DNode node)
    {
        public int FindAbsolutTextOffset(CharNode child)
        {
            var walker = new TreeWalker<TextNode>(node, child.IsDeleted);

            var offset = 0;
            var current = walker.NextNode();
            while (current is not null)
            {
                if (current.Id == child.Parent!.Id)
                {
                    break;
                }
                
                if (!current.IsDeleted)
                {
                    offset += current.Length;
                }

                current = walker.NextNode();
            }

            var relativeOffset = current?.IndexOf(child);
            if (relativeOffset is null or -1)
            {
                throw new InvalidOperationException("Can not find absolut offset of node. Probably the node is node a child of given parent.");
            }
            
            return offset + relativeOffset.Value;
        }
        
        public int FindAbsolutTextOffset(TextNode child)
        {
            var walker = new TreeWalker<TextNode>(node, child.IsDeleted);

            var offset = 0;
            var current = walker.NextNode();
            while (current is not null)
            {
                if (current.Id == child.Id)
                {
                    break;
                }
                
                if (!current.IsDeleted)
                {
                    offset += current.Length;
                }
                
                current = walker.NextNode();
            }

            if (current is null)
            {
                throw new InvalidOperationException("Can not find absolut offset of node. Probably the node is node a child of given parent.");
            }
            
            return offset;
        }
        
        public int FindAbsolutTextOffset<TNode>(DNode child) where TNode : IDNode
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
}