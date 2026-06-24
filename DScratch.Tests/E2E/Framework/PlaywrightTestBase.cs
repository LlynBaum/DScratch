using System.Text.Json;
using DScratch.Interactions;
using Microsoft.Playwright;

namespace DScratch.Tests.E2E.Framework;

[TestFixture]
public class PlaywrightTestBase : E2ETestsRunnerBase
{
    protected ILocator Editor => Page.Locator("div[contenteditable]");

    protected MenuBarLocator MenuBar => new MenuBarLocator(Page.Locator("nav.menu"));
    
    [SetUp]
    public async Task NavigateToEditor()
    {
        // Wait for the Blazor WASM engine to fully load and hydrate the editor
        await Editor.WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    protected async Task<SelectionInfo> GetCursorPositionAsync()
    {
        var selection = await Page.EvaluateAsync<JsonElement>("window.editor.getSelection()");
        return selection.Deserialize<SelectionInfo>() // Use System.Text, so direction string can be deserialized
               ?? throw new InvalidOperationException("Failed to deserialize SelectionInfo.");
    }
}