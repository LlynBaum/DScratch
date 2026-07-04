using DScratch.LayoutEngine.Pages;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.LayoutEngine;

internal sealed class LayoutEngineService(IDScratchService dScratchService, ILayoutRenderer layoutRenderer) : ILayoutEngineService
{
    private List<DPage> pages = [];
    private Dictionary<DNode, RenderInfo> nodes = new Dictionary<DNode, RenderInfo>();

    public async Task LayoutAsync(TransactionResult transactionResult)
    {
        if (transactionResult.IsEmpty) return;

        var modifiedPages = transactionResult.ModifiedNodes
            .Select(m => this.nodes.GetValueOrDefault(m.Node)?.CurrentPage)
            .Where(p => p is not null)
            .Cast<DPage>()
            .OrderBy(p => p.PageNumber)
            .ToHashSet();

        List<ElementNode> nodes = [];
        // TODO: layout

        // TODO: build rerender model and render with razor in client project
        await layoutRenderer.RenderAsync(nodes, transactionResult.CursorPosition);
    }
}