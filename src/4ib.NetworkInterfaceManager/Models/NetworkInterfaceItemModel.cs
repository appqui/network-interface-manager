using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4ib.NetworkInterfaceManager.Models
{
    public class NetworkInterfaceItemModel
    {
        [JsonProperty("ip4address")]
        public string IP4Address { get; set; }
        public string SubnetMask { get; set; }
    }
}
