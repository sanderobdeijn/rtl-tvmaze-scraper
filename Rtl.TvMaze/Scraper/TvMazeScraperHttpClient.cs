using System;
using System.Net.Http;
using Microsoft.Extensions.Options;

namespace Rtl.TvMaze.Scraper
{
    public class TvMazeScraperHttpClient
    {
        public HttpClient Client { get; }

        public TvMazeScraperHttpClient(HttpClient client, IOptions<TvMazeOptions> options)
        {
            Client = client;
            Client.BaseAddress = new Uri(options.Value?.TvMazeUrl ?? "https://api.tvmaze.com");
        }
    }
}