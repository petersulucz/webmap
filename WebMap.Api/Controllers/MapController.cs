using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Webmap.Data;
using WebMap.Api.Models;
using Webmap.Common;
using WebMap.Api.Services;
using Webmap.Common.Primitives;

namespace WebMap.Api.Controllers
{
    [Route("api/[controller]")]
    public class MapController : Controller
    {
        private readonly IMapProvider mapProvider;

        public MapController(IMapProvider mapProvider)
        {
            this.mapProvider = mapProvider;
        }

        // GET api/values
        [HttpGet("{type}")]
        public MapData Get(string type, double latitudeMin, double latitudeMax, double longitudeMin, double longitudeMax)
        {
            var low = new Vector2(-122.2093000, 47.6001000);
            var high = new Vector2(-122.1787000, 47.6289000);

            return this.mapProvider.GetMapData(type, new Vector2(longitudeMin, latitudeMin), new Vector2(longitudeMax, latitudeMax));
        }

        [HttpGet("types")]
        public IEnumerable<string> GetTypes()
        {
            return this.mapProvider.GetProviders();
        }

        [HttpGet("dimension")]
        public MapDimension GetDimension()
        {
            return this.mapProvider.GetDimension();
        }
    }
}
