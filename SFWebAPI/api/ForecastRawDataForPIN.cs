using System;
using System.Net.Http;
using System.Threading;

namespace api
{
    /// <summary>
    /// Wrapper model class for refreshing forecast data.
    /// </summary>
    public class ForecastRawDataForPIN
    {
        #region Data Members
        public DateTime NextRefresh { get; set; }

        public string ForecastDataJson { get; set; }

        #endregion

        /// <summary>
        /// Fetch the forecast data for given pin from openweather site.
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static ForecastRawDataForPIN GetLatestData(long pin, CancellationToken cancellationToken)
        {
            Uri forecastUri = OpenWeatherUri.GetForcastUriForPIN(pin);

            Telemetry.Client.TrackTrace($"Refreshing forecast data for the PIN {pin}");

            HttpClient httpClient = new HttpClient();
            var response = httpClient.GetAsync(
                forecastUri,
                cancellationToken)
                .GetAwaiter()
                .GetResult();

            if(response.IsSuccessStatusCode)
            {
                Telemetry.Client.TrackTrace($"Successfully fetched forecast data for the PIN {pin}");

                return new ForecastRawDataForPIN
                {
                    // Next refresh will be after 3 hours.
                    // TODO: Should we refresh earlier ??
                    NextRefresh = DateTime.UtcNow.AddHours(1),
                    ForecastDataJson = response.Content.ReadAsStringAsync().Result
                };
            }
            else
            {
                Telemetry.Client.TrackTrace($"Error fetching forecast data for the PIN {pin}");
                // TODO: Log error.
                return new ForecastRawDataForPIN
                {
                    NextRefresh = DateTime.MinValue,
                    ForecastDataJson = string.Empty
                };

            }
        }
    }
}
