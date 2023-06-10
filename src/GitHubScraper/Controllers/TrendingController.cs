// -----------------------------------------------------------------------
// <copyright file= "TrendingController.cs">
//     Copyright (c) Danvic.Wang All rights reserved.
// </copyright>
// Author: Danvic.Wang
// Created DateTime: 2023-06-10 09:49
// Modified by:
// Description:
// -----------------------------------------------------------------------

using GitHubScraper.Dtos;
using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;

namespace GitHubScraper.Controllers;

/// <summary>
/// Github Trending Scraper
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class TrendingController : ControllerBase
{
    #region Initializes

    private readonly IBrowserFetcher _browserFetcher;

    public TrendingController(IBrowserFetcher browserFetcher)
    {
        _browserFetcher = browserFetcher;
    }

    #endregion

    #region APIs

    /// <summary>
    /// Scrap Repositories Trending
    /// </summary>
    /// <param name="requestDto"></param>
    /// <returns></returns>
    [HttpGet(Name = nameof(GetRepositories))]
    public async Task<IList<RepositoryDto>> GetRepositories([FromQuery] RepositoryTrendingRequestDto requestDto)
    {
        await _browserFetcher.DownloadAsync();
        
        var launchOptions = new LaunchOptions
        {
            Headless = true, // Run in headless mode
            Args = new[] { "--no-sandbox" } // Optional: Add --no-sandbox argument for Linux
        };

        await using var browser = await Puppeteer.LaunchAsync(launchOptions);
        await using var page = await browser.NewPageAsync();
        
        await InterceptRequestAsync(page);
        
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

        return repositories;
    }

    private async Task InterceptRequestAsync(IPage page)
    {
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
    }

    [HttpGet("developers", Name = nameof(GetDevelopers))]
    public async Task<IList<RepositoryDto>> GetDevelopers()
    {
        return new List<RepositoryDto>();
    }

    #endregion
}