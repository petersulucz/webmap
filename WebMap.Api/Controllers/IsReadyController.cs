using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMap.Api.Services;

namespace WebMap.Api.Controllers
{
    [Route("api/[controller]")]
    public class IsReadyController : Controller
    {

        private readonly IMapProvider mapProvider;
        public IsReadyController(IMapProvider mapProvider)
        {
            this.mapProvider = mapProvider;
        }

        [HttpGet]
        public bool IsReady()
        {
            return this.mapProvider.IsReady();
        }
    }
}
