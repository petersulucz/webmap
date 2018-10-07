using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Webmap.Backend;
using Webmap.Common;
using Webmap.Data;
using WebMap.Api.Services;

namespace WebMap.Api
{
    internal class MapReader : IHostedService
    {
        private readonly IMapProvider mapProvider;

        private string sourceFile;

        public MapReader(IConfiguration configuration, IMapProvider mapProvider)
        {
            this.sourceFile = configuration.GetSection("map").GetValue<string>("file");
            this.mapProvider = mapProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (false == cancellationToken.IsCancellationRequested)
            {
                var map = await this.LoadMap();
                this.mapProvider.SetMap(map);

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(60), cancellationToken);
                }
                catch(TaskCanceledException)
                {
                    return;
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task<MapProviderController> LoadMap()
        {
            await Task.Yield();

            using (var stream = File.OpenRead(this.sourceFile))
            {
                var definition = OpenStreetMapParser.Parse(stream);

                return MapProviderController.Create(definition);
            }
        }
    }
}
