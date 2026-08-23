using System.Diagnostics;

namespace DScratch.E2E;

[SetUpFixture]
[Parallelizable(ParallelScope.Fixtures)]
public class E2ETestFixture
{
    private static Process? serverProcess;
    public const string BaseUrl = "http://127.0.0.1:5001";

    [OneTimeSetUp]
    public async Task StartServer()
    {
        var repoRoot = FindRepoRoot();
        var hostCsproj = Path.Combine(repoRoot, "src", "DScratch.Host", "DScratch.Host.csproj");

        // Force the app to run in a test profile if needed
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project \"{hostCsproj}\" --urls {BaseUrl}",
            WorkingDirectory = repoRoot,
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
        
        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(2);
        var stopwatch = Stopwatch.StartNew();
        var serverReady = false;

        // Poll until server responds
        while (stopwatch.Elapsed < TimeSpan.FromSeconds(30))
        {
            if (serverProcess.HasExited)
            {
                throw new Exception($"Server exited prematurely with code {serverProcess.ExitCode}. Check the STDOUT logs above!");
            }

            try
            {
                var response = await httpClient.GetAsync(BaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    serverReady = true;
                    break;
                }
            }
            catch
            {
                await Task.Delay(250);
            }
        }

        if (!serverReady)
        {
            throw new TimeoutException($"Server failed to start and respond at {BaseUrl} within 30 seconds.");
        }
    }

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "DScratch.slnx")) ||
                File.Exists(Path.Combine(dir.FullName, "DScratch.sln")) ||
                Directory.Exists(Path.Combine(dir.FullName, "src", "DScratch.Host")))
            {
                return dir.FullName;
            }
            dir = dir.Parent;
        }
        throw new InvalidOperationException($"Could not find repository root starting from '{AppContext.BaseDirectory}'.");
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