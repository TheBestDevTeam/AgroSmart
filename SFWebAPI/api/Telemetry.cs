using Microsoft.ApplicationInsights;

namespace api
{
    public static class Telemetry
    {
        private static TelemetryClient client = null;

        public static TelemetryClient Client
        {
            get
            {
                if (client == null)
                {
                    client = new TelemetryClient();
                    if(string.IsNullOrWhiteSpace(client.InstrumentationKey))
                    {
                        client.InstrumentationKey = "012adba5-b98d-498a-b485-2b711e4bd2aa";
                    }
                }

                return client;
            }
        }
    }
}
