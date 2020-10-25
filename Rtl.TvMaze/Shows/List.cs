using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Rtl.TvMaze.Shows
{
    public class List : Controller
    {
        private readonly IShowsService showsService;

        public List(IShowsService showsService)
        {
            this.showsService = showsService;
        }

        [AllowAnonymous]
        [HttpGet("api/show/list")]
        [SwaggerOperation(
            Summary = "List all Shows",
            Description = "List all Shows",
            OperationId = "Show.List",
            Tags = new[] { "Show" })
        ]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ShowViewModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Endpoint(int page = 0, int pageSize = 250)
        {
            var shows = showsService.GetShows(page, pageSize);
            return Ok(shows);
        }
    }
}