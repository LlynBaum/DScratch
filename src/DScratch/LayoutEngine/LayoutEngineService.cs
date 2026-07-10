using DScratch.LayoutEngine.Pages;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.LayoutEngine;

internal sealed class LayoutEngineService(ILayoutRenderer layoutRenderer) : ILayoutEngineService
{
    private readonly List<DPage> pages = [];
    private readonly Dictionary<DNode, RenderInfo> nodes = new Dictionary<DNode, RenderInfo>();

    public void AddRoot(DNode documentRoot)
    {
        var page = new DPage
        {
            PageNumber = 1,
            LastNode = documentRoot.LastChild ??
                       throw new InvalidOperationException("Document must have at least one paragraph to start with.")
        };
        
        pages.Add(page);
        nodes[documentRoot] = RenderInfo.Create(documentRoot, page);
    }

    public async Task RenderAsync(DScratchDocument document, TransactionResult transactionResult)
    {
        if (transactionResult.IsEmpty) return;

        HashSet<DPage> modifiedPages = [];

        foreach (var modifiedNode in transactionResult.ModifiedNodes)
        {
            var node = modifiedNode.Modification switch
            {
                Modification.Insert => modifiedNode.Node.Parent!,
                Modification.Delete => modifiedNode.Node,
                Modification.Changed => modifiedNode.Node,
                _ => throw new ArgumentOutOfRangeException(nameof(transactionResult), "Unknown Modification type.")
            };

            modifiedPages.Add(nodes[node].CurrentPage);
        }

        var firstPage = modifiedPages.MinBy(p => p.PageNumber) ?? pages[0];
        var previousPageIndex = firstPage.PageNumber - 2; // -1 for index and -1 for previous page
        var startNode = previousPageIndex >= 0 
            ? pages[firstPage.PageNumber - 2].LastNode 
            : null;

        var current = startNode?.RightOrigin ?? document.Root.FirstChild;
        var root = ElementNode.Root(document.Root);
        BuildLayout(root, current, firstPage);
        await layoutRenderer.RenderAsync(root, transactionResult.CursorPosition, firstPage.PageNumber);
    }

    private void BuildLayout(ElementNode parent, DNode? current, DPage currentPage)
    {
        var node = current;
        while (node is not null)
        {
            if (node.IsDeleted)
            {
                nodes.Remove(node);
                node = node.RightOrigin;
                continue;
            }

            var elementNode = ElementNode.Create(node);
            parent.ChildNodes!.Add(elementNode);

            if (!nodes.TryGetValue(node, out var info))
            {
                info = RenderInfo.Create(node, currentPage);
                nodes.Add(node, info);
            }

            info.CurrentPage = currentPage;

            if (node.FirstChild is not null && elementNode.HasChildNodes)
            {
                BuildLayout(elementNode, node.FirstChild, currentPage);
            }

            node = node.RightOrigin;
        }
    }
}