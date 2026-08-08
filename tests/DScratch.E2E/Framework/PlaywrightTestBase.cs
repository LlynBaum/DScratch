using System.Text.Json;
using DScratch.Interactions;
using Microsoft.Playwright;

namespace DScratch.E2E.Framework;

public class PlaywrightTestBase : E2ETestsRunnerBase
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };
    
    protected ILocator Editor => Page.Locator("div[contenteditable]");

    protected EditorMenuLocator EditorMenu => new EditorMenuLocator(Page.Locator("nav.menu"));
    
    [SetUp]
    public async Task NavigateToEditor()
    {
        // Wait for the Blazor WASM engine to fully load and hydrate the editor
        await Editor.WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    protected async Task<SelectionInfo?> GetCursorPositionAsync()
    {
        var selection = await Page.EvaluateAsync<JsonElement?>("window.editor.getEditorSelection()");
        if (!selection.HasValue || selection.Value.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        return selection.Value.Deserialize<SelectionInfo>(JsonSerializerOptions);
    }
}