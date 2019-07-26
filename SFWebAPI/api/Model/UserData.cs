using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace api.Model
{
    [DataContract]
    public class UserData
    {
        [DataMember(Name = "id", IsRequired = true)]
        public string Id;

        [DataMember(Name = "pin")]
        public long Pin;

        [DataMember(Name = "deviceList")]
        public List<string> DeviceList;

        [DataMember(Name = "forecastData")]
        public object ForecastDataJson;
    }
}
