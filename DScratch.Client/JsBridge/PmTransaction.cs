using Microsoft.JSInterop;

namespace DScratch.Client.JsBridge;

public class PmTransaction(IJSRuntime jsRuntime)
{
    private const string UserSelectionJsMethod = "getUserSelection";
    private const string TransactionJsMethod = "dispatchCSharpTransaction";

    private readonly List<PmStep> steps = [];

    private readonly Lazy<PmSelection> editorState = new Lazy<PmSelection>(() => LoadState(jsRuntime));
    public PmSelection EditorState => editorState.Value;

    public PmTransaction Replace() // TODO: slice param
    {
        AddStep("replace");
        return this;
    }
    
    public PmTransaction Replace(int from, int? to = null) // TODO: slice param
    {
        AddStep("replace", new Dictionary<string, object?>
        {
            { "from", from },
            { "to", to }
        });

        return this;
    }

    public PmTransaction ReplaceWith(PmNode node)
    {
        AddStep("replaceWith", new Dictionary<string, object?>
        {
            { "node", node }
        });
        return this;
    }
    
    public PmTransaction ReplaceWith(PmNode node, int from, int? to = null)
    {
        AddStep("replaceWith", new Dictionary<string, object?>
        {
            { "node", node },
            { "from", from },
            { "to", to }
        });
        return this;
    }
    
    public async Task DispatchAsync()
    {
        await jsRuntime.InvokeVoidAsync(TransactionJsMethod, steps);
    }

    private static PmSelection LoadState(IJSRuntime jsRuntime)
    {
        return jsRuntime.GetValueAsync<PmSelection>(UserSelectionJsMethod)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult(); // TODO: bad, very bad, but what else then?
    }

    private void AddStep(string name, Dictionary<string, object?>? args = null)
    {
        steps.Add(new PmStep(name, args ?? []));
    }

    private record PmStep(string Name, Dictionary<string, object?> Args);
    
    public class PmSelection
    {
        public int From { get; }
            
        public int To { get; }
    }
}