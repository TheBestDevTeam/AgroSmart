using System.Runtime.Serialization;

namespace api.Model
{
    [DataContract]
    public class DeviceData
    {
        [DataMember(Name = "id")]
        public string Id;

        [DataMember(Name = "temperature")]
        public string Temperature;

        [DataMember(Name = "humidity")]
        public string Humidity;

        [DataMember(Name = "moisture")]
        public string Moisture;
    }

    [DataContract]
    public class DeviceStatus
    {
        [DataMember(Name = "id")]
        public string Id;

        [DataMember(Name = "status")]
        public string Status;
    }
}
