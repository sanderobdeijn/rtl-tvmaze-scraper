using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Rtl.TvMaze.Scraper
{
    public class ScraperNode : IScraperNode 
    {
        private readonly TvMazeScraperHttpClient tvMazeScraperClient;

        public ScraperNode(TvMazeScraperHttpClient tvMazeScraperClient)
        {
            this.tvMazeScraperClient = tvMazeScraperClient;
        }

        public async Task<IEnumerable<ShowDto>> GetShowsForPage(int page)
        {
            var shows = new List<ShowDto>();

            try
            {
                var response = await tvMazeScraperClient.Client.GetAsync($"/shows?page={page}");

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();
                
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    // Check if we actually hit rate limiting. According to the documentation with the show endpoint always uses the edge cache and isn't rate limited
                    Console.WriteLine(result);
                }

                shows.AddRange(JsonConvert.DeserializeObject<IEnumerable<ShowDto>>(result));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return shows;
        }

        public async Task<IEnumerable<ActorDto>> GetActorsForShow(int tvMazeId)
        {
            var actors = new List<ActorDto>();

            try
            {
                var response = await tvMazeScraperClient.Client.GetAsync($"/shows/{tvMazeId}/cast");

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    // Check if we actually hit rate limiting. According to the documentation with the show endpoint always uses the edge cache and isn't rate limited
                    Console.WriteLine(result);
                }

                var persons = JsonConvert.DeserializeObject<IEnumerable<PersonDto>>(result);
                actors.AddRange(persons.Select(p => p.Person));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return actors;
        }
    }
}