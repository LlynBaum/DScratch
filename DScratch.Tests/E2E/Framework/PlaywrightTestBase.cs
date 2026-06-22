using DScratch.Interactions;
using Microsoft.Playwright;

namespace DScratch.Tests.E2E.Framework;

[TestFixture]
[Parallelizable(ParallelScope.Self)] // TODO: this caused problems, and suddenly had ghost processes running. Maybe double check if this is actually save to do.
public class PlaywrightTestBase : E2ETestsRunnerBase
{
    protected ILocator Editor => Page.Locator("div[contenteditable]");

    protected ILocator MenuBar => Page.Locator("nav.menu");
    
    [SetUp]
    public async Task NavigateToEditor()
    {
        await Page.GotoAsync(E2ETestFixture.BaseUrl);
        // Wait for the Blazor WASM engine to fully load and hydrate the editor
        await Editor.WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    protected async Task<SelectionInfo> GetCursorPositionAsync()
    {
        return await Page.EvaluateAsync<SelectionInfo>("getSelection()");
    }
}