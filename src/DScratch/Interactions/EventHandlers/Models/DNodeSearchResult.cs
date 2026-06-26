using DScratch.Nodes;

namespace DScratch.Interactions.EventHandlers.Models;

public sealed record DNodeSearchResult(DNodeInfo Origin, DNodeInfo RightOrigin);

public record NodeSearchResult<TNode>(NodeInfo<TNode> Origin, NodeInfo<TNode> RightOrigin) where TNode : DNode;