using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;

namespace DScratch;

public class TreeWalker<TFilter>(DNode parent) : IDisposable where TFilter : IDNode
{
    private IEnumerator<DNode> enumerator = parent.ActiveChildNodes.GetEnumerator();
    
    public TFilter? Current { get; private set; }

    public void MoveNext()
    {
        while (enumerator.MoveNext())
        {
            if (enumerator.Current is TFilter node)
            {
                Current = node;
                return;
            }
        }

        Current = default;
        enumerator.Dispose();
        enumerator = parent.ActiveChildNodes.GetEnumerator();
    }

    public void Dispose()
    {
        enumerator.Dispose();
    }
}