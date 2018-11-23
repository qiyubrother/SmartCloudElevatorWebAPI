using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCloudElevatorWebAPI
{
    [Serializable]
    public class In
    {
    }
    [Serializable]
    public class Out
    {
        [JsonProperty("ErrorCode")]
        public string ErrorCode { get; set; }
        [JsonProperty("ErrorMessage")]
        public string ErrorMessage { get; set; }
    }
}
