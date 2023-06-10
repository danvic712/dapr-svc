// -----------------------------------------------------------------------
// <copyright file= "RepositoryTrendingRequestDto.cs">
//     Copyright (c) Danvic.Wang All rights reserved.
// </copyright>
// Author: Danvic.Wang
// Created DateTime: 2023-06-10 10:08
// Modified by:
// Description:
// -----------------------------------------------------------------------

namespace GitHubScraper.Dtos;

/// <summary>
/// 
/// </summary>
public class RepositoryTrendingRequestDto
{
    /// <summary>
    /// 
    /// </summary>
    public string? Language { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string? SpokenLanguageCode { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string? Since { get; set; }
}