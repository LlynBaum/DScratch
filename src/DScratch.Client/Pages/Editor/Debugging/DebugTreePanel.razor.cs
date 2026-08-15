using DScratch.Client.Services;
using DScratch.Nodes;

namespace DScratch.Client.Pages.Editor.Debugging;

public partial class DebugTreePanel(IEditorDebugService debugService, IDScratchService dScratchService) : IDisposable
{
    private List<TreeNodeViewModel> treeNodes = [];
    
    protected override void OnInitialized()
    {
        debugService.DocumentChanged += OnDocumentChanged;
        debugService.DebugModeChanged += OnDebugModeChanged;
        UpdateTreeNodes();
    }

    private void OnDocumentChanged()
    {
        InvokeAsync(() =>
        {
            UpdateTreeNodes();
            StateHasChanged();
        });
    }

    private void OnDebugModeChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    private void UpdateTreeNodes()
    {
        var doc = dScratchService.Document;
        treeNodes = BuildTreeNodes(doc.Root);
    }

    private List<TreeNodeViewModel> BuildTreeNodes(DNode root)
    {
        var list = new List<TreeNodeViewModel>();
        int indent = 0;
        
        var current = root;
        var visited = new HashSet<DNode>();
        
        while (current is not null && !visited.Contains(current))
        {
            visited.Add(current);
            
            var text = current is TextNode tn ? tn.TextContent : "";
            var href = current is LinkNode ln ? ln.Href : null;
            list.Add(new TreeNodeViewModel
            {
                Id = current.Id.Value,
                Type = current.GetType().Name,
                Indent = indent,
                IsDeleted = current.IsDeleted,
                OriginId = current.Origin?.Id.Value ?? "null",
                RightOriginId = current.RightOrigin?.Id.Value ?? "null",
                TextContent = text,
                Href = href
            });
            
            if (current.ChildNodes.Count > 0)
            {
                indent += 1;
                current = current.ChildNodes[0];
            }
            else
            {
                var node = current;
                while (node is not null)
                {
                    if (node.RightOrigin is not null)
                    {
                        current = node.RightOrigin;
                        break;
                    }
                    indent -= 1;
                    node = node.Parent;
                }
                if (node is null)
                {
                    current = null;
                }
            }
        }
        return list;
    }

    public void Dispose()
    {
        debugService.DocumentChanged -= OnDocumentChanged;
        debugService.DebugModeChanged -= OnDebugModeChanged;
    }
}

public class TreeNodeViewModel
{
    public string Id { get; set; } = "";
    public string Type { get; set; } = "";
    public int Indent { get; set; }
    public bool IsDeleted { get; set; }
    public string OriginId { get; set; } = "";
    public string RightOriginId { get; set; } = "";
    public string TextContent { get; set; } = "";
    public string? Href { get; set; }
}
