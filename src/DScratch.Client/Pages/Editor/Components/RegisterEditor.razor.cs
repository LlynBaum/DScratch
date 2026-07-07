using System.Runtime.InteropServices;
using DScratch.Client.BrowserInteractions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DScratch.Client.Pages.Editor.Components;

public partial class RegisterEditor(
    InputEventHelper inputEventHelper,
    IDScratchService dScratchService,
    DJsInvoker jsInvoker) : ComponentBase
{
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && RuntimeInformation.ProcessArchitecture == Architecture.Wasm)
        {
            var dotNetRef = DotNetObjectReference.Create(inputEventHelper);
            await jsInvoker.InitializeEditor(dotNetRef);
            await dScratchService.InitialTransactionAsync();
        }
    }
}