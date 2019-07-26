using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Data;

namespace api
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class api : StatefulService
    {
        public api(StatefulServiceContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[]
            {
                new ServiceReplicaListener(serviceContext =>
                    new KestrelCommunicationListener(
                        serviceContext,
                        "ServiceEndPoint",
                        (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        return new WebHostBuilder()
                                    .UseKestrel()
                                    .ConfigureServices(
                                        services => services
                                            .AddSingleton<StatefulServiceContext>(serviceContext)
                                            .AddSingleton<IReliableStateManager>(this.StateManager))
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseStartup<Startup>()
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url)
                                    .Build();
                    }))
            };
        }

        /// <summary>
        /// Background job to refresh the forecast data for the PINs in use.
        /// Every 5mins it tries to refresh the forecast data, and if any of
        /// the PIN forecast data next refresh interval is met, the the latest
        /// data will be fetched from the OpenWeather site, and stored in the
        /// reliable dictionary.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            while(true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    await ReliableCollectionHelper.RefreshForecastDataAsync(this.StateManager,cancellationToken);
                }
                catch (TimeoutException te)
                {
                    // This could be due to the reliable collections might not be
                    // able to get a lock on the persisten storage and timedout.
                    // Ignore the timeout exception in this case and retry after
                    // the sleep.

                    Telemetry.Client.TrackException(te);
                }
                catch(Exception ex)
                {
                    // Ignore the exception, but log it.
                    if (this.Context != null)
                    {
                        ServiceEventSource.Current.ServiceMessage(
                            this.Context,
                            $"Caught exception refreshing forecast data: {ex}");
                    }

                    Telemetry.Client.TrackException(ex);
                }

                await Task.Delay(TimeSpan.FromMinutes(2), cancellationToken);
            }
        }
    }
}
