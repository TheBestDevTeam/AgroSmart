using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace api.Model
{
    [DataContract]
    public class ForecastDataForPIN
    {
        [DataMember(Name = "pin")]
        public long Pin;

        [DataMember(Name = "forecastdata")]
        public string ForecastDataJson;

        //TODO: Define memebers as per the forecast data fields.
    }
}
