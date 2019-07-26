using System;
using System.Text;

namespace api
{
    //// Open Weather API Uri format
    //// https://api.openweathermap.org/data/2.5/forecast?zip=500089,IN&units=metric&appid=232439050c541defc0696cb23a1e1ae2
    /// <summary>
    /// OpenWeather URI constants wrapper class.
    /// </summary>
    public static class OpenWeatherUri
    {
        const string ForeCastBaseUri = @"https://api.openweathermap.org/data/2.5/forecast";

        // PIN-code,IN
        const string ZipFormatIndia = "{0},IN";

        public static class QueryKeys
        {
            internal const string Zip = "zip";
            internal const string Units = "units";
            internal const string AppId = "appid";
        }

        public static class QueryValue
        {
            internal const string Metric = "metric";
            internal const string AppId = "232439050c541defc0696cb23a1e1ae2";

            internal static string GetZip(long pin)
            {
                return string.Format(ZipFormatIndia, pin);
            }
        }

        public static Uri GetForcastUriForPIN(long pin)
        {
            var openWeatherUri = new StringBuilder();
            openWeatherUri.Append(ForeCastBaseUri);
            openWeatherUri.AppendFormat("?{0}={1}", QueryKeys.Zip, QueryValue.GetZip(pin));
            openWeatherUri.AppendFormat("&{0}={1}", QueryKeys.Units, QueryValue.Metric);
            openWeatherUri.AppendFormat("&{0}={1}", QueryKeys.AppId, QueryValue.AppId);

            return new Uri(openWeatherUri.ToString());
        }
    }

    public static class ReliableObjectNames
    {
        public const string ForecastDataDictionary = "ForecastData";
        public const string DeviceDataDictonary = "DeviceData";
        public const string UserDataDictionary = "UserData";
        public const string DeviceStatusDictionary = "DeviceStatus";
    }
}