using System.Reflection;
using Microsoft.OpenApi.Models;
using PuppeteerScraper.Dtos;
using PuppeteerSharp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IBrowserFetcher, BrowserFetcher>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Puppeteer Scraper API",
        Description = "An ASP.NET Core API for scrapping resources",
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://github.com/danvic712/samples/blob/main/LICENSE")
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Puppeteer Scraper API v1");
    options.RoutePrefix = string.Empty;
});

app.MapGet("/github/trending", GetGitHubTrendingAsync)
    .WithName("Github Trending Scraper")
    .WithTags("GitHub Scraper")
    .WithOpenApi();

app.Run();

static async Task<IResult> GetGitHubTrendingAsync(IBrowserFetcher browserFetcher)
{
    await browserFetcher.DownloadAsync();

    var launchOptions = new LaunchOptions
    {
        Headless = true, // Run in headless mode
        Args = new[] { "--no-sandbox" } // Optional: Add --no-sandbox argument for Linux
    };

    await using var browser = await Puppeteer.LaunchAsync(launchOptions);
    await using var page = await browser.NewPageAsync();

    // Set a smaller viewport size for the page, A smaller viewport reduces the amount of content that needs to be rendered
    await page.SetViewportAsync(new ViewPortOptions { Width = 1280, Height = 720 });

    await page.SetRequestInterceptionAsync(true);
    page.Request += async (sender, e) =>
    {
        // Intercept requests to download pages only
        if (e.Request.ResourceType == ResourceType.Document)
        {
            await e.Request.ContinueAsync();
        }
        else
        {
            await e.Request.AbortAsync();
        }
    };
        
    // Navigate to GitHub Trending
    await page.GoToAsync("https://github.com/trending");

    // Perform scraping operations
    var repositories = await page.EvaluateExpressionAsync<List<RepositoryDto>>(@"
            Array.from(document.querySelectorAll('article.Box-row')).map(repo => ({
                title: repo.querySelector('h2').innerText.trim(),
                description: repo.querySelector('p')?.innerText.trim(),
                link: repo.querySelector('h2 a').href.trim(),
                language: repo.querySelector(['div span span[itemprop=""programmingLanguage""]'])?.innerText.trim(),
                stars: repo.querySelector(['div a[href*=""stargazers""]'])?.innerText.trim(),
                forks: repo.querySelector(['div a[href*=""forks""]'])?.innerText.trim()
            }))
        ");

    // Close the browser instance
    await browser.CloseAsync();
    
    return TypedResults.Ok(repositories);
}