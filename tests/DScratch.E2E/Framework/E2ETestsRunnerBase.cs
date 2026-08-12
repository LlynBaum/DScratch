using Microsoft.Playwright;

namespace DScratch.E2E.Framework;

public class E2ETestsRunnerBase
{
    private const int DefaultTimeoutSec = 2;
    
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
        SetDefaultExpectTimeout(DefaultTimeoutSec * 1000);
        
        await Page.GotoAsync(E2ETestFixture.BaseUrl);
    }

    [TearDown]
    public async Task TearDown() {
        await Page.CloseAsync();
    }

    [OneTimeTearDown]
    public async Task GlobalTearDown()
    {
        await browser.DisposeAsync();
        playwright.Dispose();
    }
}