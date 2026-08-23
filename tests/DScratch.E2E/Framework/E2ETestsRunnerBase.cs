using Microsoft.Playwright;

namespace DScratch.E2E.Framework;

public class E2ETestsRunnerBase
{
    private const int DefaultTimeoutSec = 2;
    
    protected virtual bool EnableTracing { get; } = false;
    
    private IPlaywright playwright = null!;
    private IBrowser browser = null!;
    protected IPage Page { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        playwright = await Playwright.CreateAsync();
        browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            ExecutablePath = "/usr/bin/chromium-browser",
            Headless = true
        });
    }

    [SetUp]
    public async Task Setup()
    {
        Page = await browser.NewPageAsync();
        if (EnableTracing)
        {
            await Page.Context.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
        }
        
        SetDefaultExpectTimeout(DefaultTimeoutSec * 1000);
        
        await Page.GotoAsync(E2ETestFixture.BaseUrl);
    }

    [TearDown]
    public async Task TearDown() {
        if (EnableTracing)
        {
            var failed = TestContext.CurrentContext.Result.Outcome == NUnit.Framework.Interfaces.ResultState.Error
                         || TestContext.CurrentContext.Result.Outcome == NUnit.Framework.Interfaces.ResultState.Failure;

            var path = failed ? Path.Combine(
                TestContext.CurrentContext.WorkDirectory,
                "playwright-traces",
                $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}.zip"
            ) : null;
        
            await Page.Context.Tracing.StopAsync(new TracingStopOptions
            {
                Path = path
            });

            if (path is not null)
            {
                await TestContext.Out.WriteLineAsync($"Traces at '{path}'");
            }
        }
        
        await Page.CloseAsync();
    }

    [OneTimeTearDown]
    public async Task GlobalTearDown()
    {
        await browser.DisposeAsync();
        playwright.Dispose();
    }
}