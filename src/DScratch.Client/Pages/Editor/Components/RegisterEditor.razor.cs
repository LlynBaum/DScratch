using System.Runtime.InteropServices;
using DScratch.Client.BrowserInteractions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DScratch.Client.Pages.Editor.Components;

public partial class RegisterEditor(
    BrowserEventHelper browserEventHelper,
    IDScratchService dScratchService,
    DJsInvoker jsInvoker) : ComponentBase
{
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && RuntimeInformation.ProcessArchitecture == Architecture.Wasm)
        {
            var initialTransaction = dScratchService.InitialTransaction();
            await jsInvoker.ApplyTransaction(initialTransaction);
            
            var dotNetRef = DotNetObjectReference.Create(browserEventHelper);
            await jsInvoker.InitializeEditor(dotNetRef);
        }
    }
}