using Microsoft.JSInterop;

namespace DScratch.Client.Updates;

public class PmTransaction(IJSRuntime jsRuntime)
{
    private const string TransactionJsMethod = "dispatchCSharpTransaction";

    private readonly Lazy<PmSelection> editorState = new Lazy<PmSelection>(() => LoadState(jsRuntime));
    public PmSelection EditorState => editorState.Value;
    
    public async Task DispatchAsync()
    {
        await jsRuntime.InvokeVoidAsync(TransactionJsMethod, new Transaction());
    }

    private static PmSelection LoadState(IJSRuntime jsRuntime)
    {
        return jsRuntime.GetValueAsync<PmSelection>("getSelection")
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    private record Transaction();

    private record Step();
    
    public class PmSelection
    {
        public int From { get; }
            
        public int To { get; }
    }
}