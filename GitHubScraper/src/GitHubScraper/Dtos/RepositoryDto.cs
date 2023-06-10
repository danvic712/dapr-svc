// -----------------------------------------------------------------------
// <copyright file= "RepositoryDto.cs">
//     Copyright (c) Danvic.Wang All rights reserved.
// </copyright>
// Author: Danvic.Wang
// Created DateTime: 2023-06-10 09:54
// Modified by:
// Description:
// -----------------------------------------------------------------------

namespace GitHubScraper.Dtos;

public class RepositoryDto
{
    public string Title { get; set; }
    public string Link { get; set; } = "";
    public string Description { get; set; } = "";
    public string Language { get; set; } = "";
    public string Stars { get; set; }
    public string Forks { get; set; }
}