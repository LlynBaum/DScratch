namespace DScratch.Nodes;

public static class DNodeExtension
{
    extension(DNode node)
    {
        public int GetAbsolutTextOffset(DNode child)
        {
            var walker = new TreeWalker<TextNode>(node);

            var offset = 0;
            var current = walker.NextNode();
            while (current is not null)
            {
                if (current.Id == child.Id)
                {
                    break;
                }

                offset = current.Length;
                current= walker.NextNode();
            }

            return offset;
        }
    }
}