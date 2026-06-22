using System.Diagnostics;

namespace DScratch.Tests.E2E;

[SetUpFixture]
public class E2ETestFixture
{
    private static Process? serverProcess;
    public const string BaseUrl = "http://localhost:5001";

    [OneTimeSetUp]
    public void StartServer()
    {
        // Force the app to run in a test profile if needed
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "run --project ../../../../DScratch.Server/DScratch.Server.csproj --urls " + BaseUrl,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        serverProcess = Process.Start(startInfo);
        
        // Give the WASM app a few seconds to boot up and establish its server bindings
        Thread.Sleep(3000); 
    }

    [OneTimeTearDown]
    public void StopServer()
    {
        if (serverProcess is { HasExited: false })
        {
            serverProcess.Kill(entireProcessTree: true);
            serverProcess.Dispose();
        }
    }
}