using DScratch.Nodes;

namespace DScratch.Interactions.EventHandlers.Models;

public sealed record DNodeSearchResult(DNodeInfo Origin, DNodeInfo RightOrigin)
{
    public static DNodeSearchResult Empty { get; } = new DNodeSearchResult(
        Origin: DNodeInfo.NotFound(), 
        RightOrigin: DNodeInfo.NotFound());
}

public record NodeSearchResult<TNode>(NodeInfo<TNode> Origin, NodeInfo<TNode> RightOrigin) where TNode : DNode;