using Microsoft.JSInterop;

namespace DScratch.Client.JsBridge;

public class PmTransaction(IJSRuntime jsRuntime)
{
    private const string UserSelectionJsMethod = "getUserSelection";
    private const string TransactionJsMethod = "dispatchCSharpTransaction";

    private readonly List<PmStep> steps = [];

    private PmSelection? editorState;

    /// <summary>
    /// Get the current selection of the User.
    /// </summary>
    /// <remarks>Does not update when updating steps.</remarks>
    /// <returns></returns>
    public async Task<PmSelection> GetSelection()
    {
        editorState ??= await jsRuntime.GetValueAsync<PmSelection>(UserSelectionJsMethod);
        return editorState;
    }
    
    /// <summary>
    /// Dispatch the Transaction and commit it.
    /// </summary>
    public async Task DispatchAsync()
    {
        await jsRuntime.InvokeVoidAsync(TransactionJsMethod, steps);
    }

    /// <summary>
    /// Replace current selection with given Slice.
    /// </summary>
    /// <returns>The Transaction for chaining.</returns>
    public PmTransaction Replace(PmSlice? slice = null)
    {
        AddStep("replace", new Dictionary<string, object?>
        {
            { "slice", slice }
        });
        return this;
    }
    
    /// <summary>
    /// Replace the part of the document between from and to with the given slice.
    /// </summary>
    /// <returns>The Transaction for chaining.</returns>
    public PmTransaction Replace(int from, int? to = null, PmSlice? slice = null)
    {
        AddStep("replace", new Dictionary<string, object?>
        {
            { "from", from },
            { "to", to },
            { "slice", slice }
        });

        return this;
    }

    /// <summary>
    /// Replace the current selection with the given content, which may be a node or array of nodes.
    /// </summary>
    /// <returns>The Transaction for chaining.</returns>
    public PmTransaction ReplaceWith(params PmNode[] node)
    {
        AddStep("replaceWith", new Dictionary<string, object?>
        {
            { "nodes", node }
        });
        return this;
    }
    
    /// <summary>
    /// Replace the given range with the given content, which may be a node or array of nodes.
    /// </summary>
    /// <returns>The Transaction for chaining.</returns>
    public PmTransaction ReplaceWith(PmNode node, int from, int? to = null)
    {
        AddStep("replaceWith", new Dictionary<string, object?>
        {
            { "nodes", node },
            { "from", from },
            { "to", to }
        });
        return this;
    }

    /// <summary>
    /// Delete the content between the given positions.
    /// </summary>
    /// <returns>The Transaction for chaining.</returns>
    public PmTransaction Delete(int from, int to)
    {
        AddStep("delete", new Dictionary<string, object?>
        {
            { "from", from },
            { "to", to }
        });
        return this;
    }
    
    /// <summary>
    /// Insert the given content at the given position.
    /// </summary>
    /// <returns>The Transaction for chaining.</returns>
    public PmTransaction Insert(int position, params PmNode[] nodes)
    {
        AddStep("insert", new Dictionary<string, object?>
        {
            { "pos", position },
            { "content", nodes }
        });
        return this;
    }

    /// <summary>
    /// Replace a range of the document with a given slice, using from nad to.
    /// The similar replace method is a more primitive tool which will not move the start and end of its given range,
    /// and is useful in situations where you need more precise control over what happens.
    /// </summary>
    /// <returns>The Transaction for chaining.</returns>
    public PmTransaction ReplaceRange(int from, int to, PmSlice slice)
    {
        AddStep("replaceRange", new Dictionary<string, object?>
        {
            { "from", from },
            { "to", to },
            { "slice", slice }
        });
        return this;
    }
    
    /// <summary>
    /// Replace the given range with a node, but use from and to as hints, rather than precise positions.
    /// When from and to are the same and are at the start or end of a parent node in which the given node doesn't fit,
    /// this method may move them out towards a parent that does allow the given node to be placed.
    /// </summary>
    /// <returns>The Transaction for chaining.</returns>
    public PmTransaction ReplaceRangeWith(PmNode node, int from, int to)
    {
        AddStep("replaceRangeWith", new Dictionary<string, object?>
        {
            { "node", node },
            { "from", from },
            { "to", to }
        });
        return this;
    }
    
    /// <summary>
    /// Delete the given range, expanding it to cover fully covered parent nodes until a valid replacement is found
    /// </summary>
    /// <returns>The Transaction for chaining.</returns>
    public PmTransaction DeleteRange(int from, int to)
    {
        AddStep("deleteRange", new Dictionary<string, object?>
        {
            { "from", from },
            { "to", to }
        });
        return this;
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