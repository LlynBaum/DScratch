using System.Diagnostics;

namespace DScratch.E2E;

[SetUpFixture]
[Parallelizable(ParallelScope.Fixtures)]
public class E2ETestFixture
{
    private static Process? serverProcess;
    public const string BaseUrl = "http://127.0.0.1:5001";

    [OneTimeSetUp]
    public void StartServer()
    {
        // Force the app to run in a test profile if needed
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project ../../../../../src/DScratch.Host/DScratch.Host.csproj --urls {BaseUrl}",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        serverProcess = new Process
        {
            StartInfo = startInfo
        };

        serverProcess.OutputDataReceived += (_, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                TestContext.Progress.WriteLine($"[SERVER STDOUT] {e.Data}");
            }
        };

        serverProcess.ErrorDataReceived += (_, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                TestContext.Progress.WriteLine($"[SERVER STDERR] {e.Data}");
            }
        };

        serverProcess.Start();
        serverProcess.BeginOutputReadLine();
        serverProcess.BeginErrorReadLine();
        
        // Give the WASM app a few seconds to boot up and establish its server bindings
        Thread.Sleep(3000);

        if (serverProcess.HasExited)
        {
            throw new Exception($"Server exited prematurely with code {serverProcess.ExitCode}. Check the STDOUT logs above!");
        }
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