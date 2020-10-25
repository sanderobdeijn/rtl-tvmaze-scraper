using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rtl.TvMaze.Data;
using Rtl.TvMaze.Models;

namespace Rtl.TvMaze.Scraper
{
    public class ScraperService : BackgroundService
    {
        private readonly IServiceProvider services;
        private readonly ILogger<ScraperService> logger;

        public ScraperService(IServiceProvider services, ILogger<ScraperService> logger)
        {
            this.services = services;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Starting Scraperservice.");

            try
            {
                using (var scope = services.CreateScope())
                {
                    var worker = scope.ServiceProvider.GetRequiredService<IScraperNode>();
                    var tvMazeDbContext = scope.ServiceProvider.GetRequiredService<TvMazeDbContext>();
                    var tvMazeOptions = scope.ServiceProvider.GetRequiredService<IOptions<TvMazeOptions>>().Value;

                    await WaitForDatabaseToBeReady(tvMazeDbContext, stoppingToken);

                    var page = CalculateNextPageNumber(tvMazeDbContext, tvMazeOptions);

                    logger.LogInformation($"Getting shows for page {page}.");

                    while (true)
                    {
                        var shows = (await worker.GetShowsForPage(page));

                        if (shows == null)
                        {
                            break;
                        }

                        logger.LogInformation($"Received {shows.Count()} shows");

                        foreach (var show in shows)
                        {
                            if (await CheckIfShowExist(stoppingToken, tvMazeDbContext, show)) continue;

                            var actors = (await worker.GetActorsForShow(show.Id)).ToList();

                            var actorsToAdd = GetActorsToAdd(tvMazeDbContext, actors);

                            await AddActors(stoppingToken, tvMazeDbContext, actorsToAdd);

                            await AddShow(stoppingToken, tvMazeDbContext, show, actorsToAdd);

                            try
                            {
                                await tvMazeDbContext.SaveChangesAsync(stoppingToken);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                throw;
                            }
                        }

                        ++page;
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                logger.LogTrace(e.StackTrace);

                Console.WriteLine(e);
                throw;
            }
        }

        private static async Task AddShow(CancellationToken stoppingToken, TvMazeDbContext tvMazeDbContext, ShowDto show,
            IEnumerable<ActorDto> actorsToAdd)
        {
            await tvMazeDbContext.Set<Show>().AddAsync(new Show
            {
                Id = show.Id,
                Name = show.Name,
                ShowActors = actorsToAdd
                    .Select(actor => new ShowActor
                    {
                        ShowId = show.Id, ActorId = actor.Id
                    }).ToList()
            }, stoppingToken);
        }


        private static async Task AddActors(CancellationToken stoppingToken, TvMazeDbContext tvMazeDbContext,
            IEnumerable<ActorDto> actorsToAdd)
        {
            await tvMazeDbContext.Set<Actor>().AddRangeAsync(actorsToAdd
                .Select(actor => new Actor
                {
                    Id = actor.Id,
                    Name = actor.Name,
                    DateOfBirth = actor.BirthDay
                }), stoppingToken);
        }

        private async Task WaitForDatabaseToBeReady(TvMazeDbContext tvMazeDbContext, CancellationToken stoppingToken)
        {
            while (true)
            {
                if (await tvMazeDbContext.Database.CanConnectAsync(stoppingToken))
                {
                    break;
                }

                logger.LogInformation("Waiting for Database to be ready.");
                await Task.Delay(3000, stoppingToken);
            }
        }

        private async Task<bool> CheckIfShowExist(CancellationToken stoppingToken, TvMazeDbContext tvMazeDbContext,
            ShowDto show)
        {
            if (await tvMazeDbContext.Set<Show>().AnyAsync(s => s.Id == show.Id, stoppingToken))
            {
                logger.LogInformation($"Show {show.Id} - {show.Name} already exists.");
                return true;
            }

            return false;
        }

        private static List<ActorDto> GetActorsToAdd(TvMazeDbContext tvMazeDbContext, IEnumerable<ActorDto> actors)
        {
            var existingActorIds = tvMazeDbContext.Set<Actor>().Select(a => a.Id).ToList();
            var actorsToAdd = actors.Where(actor => !existingActorIds.Contains(actor.Id))
                .GroupBy(a => a.Id).Select(a => a.First()).ToList();
            return actorsToAdd;
        }

        private static int CalculateNextPageNumber(TvMazeDbContext tvMazeDbContext, TvMazeOptions tvMazeConfiguration)
        {
            try
            {

                var page = 0;

                if (!tvMazeDbContext.Set<Show>().Any()) return page;

                var lastAddedShowId = tvMazeDbContext.Set<Show>().OrderBy(s => s.Id).Last().Id;

                page = Convert.ToInt32(Math.Round(lastAddedShowId / tvMazeConfiguration.MaximumNumberOfShowsPerPage,
                    MidpointRounding.ToZero));

                return page;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Stopping Scraperservice.");

            await base.StopAsync(stoppingToken);
        }
    }
}