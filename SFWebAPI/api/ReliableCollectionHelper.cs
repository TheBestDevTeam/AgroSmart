using api.Model;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace api
{
    /// <summary>
    /// Wrapper calls to refresh forecast data for the PINs in use.
    /// </summary>
    public static class ReliableCollectionHelper
    {
        /// <summary>
        /// Iterates through the available PIN forecast data,
        /// and refreshes forecast data if any of the PINs next refresh time reaches.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task RefreshForecastDataAsync(
            IReliableStateManager stateManager,
            CancellationToken cancellationToken)
        {
            var forecastData =
                await stateManager.GetOrAddAsync<IReliableDictionary<long, ForecastRawDataForPIN>>(
                    ReliableObjectNames.ForecastDataDictionary);
            using (var txn = stateManager.CreateTransaction())
            {
                var enumerator = (await forecastData.CreateEnumerableAsync(txn)).GetAsyncEnumerator();
                while(await enumerator.MoveNextAsync(cancellationToken))
                {
                    long pin = enumerator.Current.Key;
                    if(pin <= 500000 || pin >= 600000)
                    {
                        Telemetry.Client.TrackTrace($"Found invalid pin {enumerator.Current.Key}, " +
                            $"removing it.",
                            Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Warning);

                        await forecastData.TryRemoveAsync(txn, pin);
                        await txn.CommitAsync();
                        break;
                    }

                    Telemetry.Client.TrackTrace($"Checking refresh schedule for the pin {enumerator.Current.Key}, "+
                        $"NextRefresh: {enumerator.Current.Value.NextRefresh}",
                        Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Warning);

                    if (string.IsNullOrWhiteSpace(enumerator.Current.Value.ForecastDataJson) ||
                        DateTime.UtcNow.CompareTo(enumerator.Current.Value.NextRefresh) >= 0 )
                    {
                        var latestData = ForecastRawDataForPIN.GetLatestData(
                            enumerator.Current.Key,
                            cancellationToken);
                        await forecastData.SetAsync(txn, enumerator.Current.Key, latestData);
                        await txn.CommitAsync();

                        Telemetry.Client.TrackTrace(
                            $"Forecast data refreshed for the pin {enumerator.Current.Key}.",
                            Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Warning);

                        // Lets do only one PIN forecast data refresh in one refresh cycle.
                        break;
                    }
                }
            }
        }

        public static object FetchForecastDataJsonObj(
            IReliableStateManager stateManager,
            long pin)
        {
            var forecastData =
                stateManager.GetOrAddAsync<IReliableDictionary<long, ForecastRawDataForPIN>>(
                    ReliableObjectNames.ForecastDataDictionary)
                    .GetAwaiter()
                    .GetResult();

            object fcData = null;
            using (var txn = stateManager.CreateTransaction())
            {
                var data = forecastData.TryGetValueAsync(txn, pin).GetAwaiter().GetResult();
                if (data.HasValue &&
                    !string.IsNullOrWhiteSpace(data.Value.ForecastDataJson))
                {
                    fcData = JsonConvert.DeserializeObject(data.Value.ForecastDataJson);
                }
                else
                {
                    fcData = "Not available";

                    Telemetry.Client.TrackTrace(
                        $"Forecast data not available for the {pin} yet.",
                        Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Warning);
                }
            }

            return fcData;
        }

        public static void AddPinToForecastCollection(
            IReliableStateManager stateManager,
            long pin)
        {
            var forecastData =
                stateManager.GetOrAddAsync<IReliableDictionary<long, ForecastRawDataForPIN>>(
                    ReliableObjectNames.ForecastDataDictionary)
                    .GetAwaiter()
                    .GetResult();

            using (var txn = stateManager.CreateTransaction())
            {
                var data = forecastData.TryGetValueAsync(txn, pin).GetAwaiter().GetResult();
                if (!data.HasValue)
                {
                    forecastData.SetAsync(
                        txn,
                        pin,
                        new ForecastRawDataForPIN()
                        {
                            ForecastDataJson = string.Empty,
                            NextRefresh = DateTime.UtcNow
                        })
                        .GetAwaiter()
                        .GetResult();

                    txn.CommitAsync().GetAwaiter().GetResult();
                }
                else
                {
                    Telemetry.Client.TrackTrace(
                        $"Forecast data for the {pin} already in tracking.",
                        Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Warning);
                }
            }
        }
    }
}
