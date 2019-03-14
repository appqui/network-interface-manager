using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace _4ib.NetworkInterfaceManager.Models
{
    public class NetworkInterfaceModel
    {
        public NetworkInterfaceModel(NetworkInterface input)
        {
            Name = input.Name;
            Description = input.Description;
            Addresses = input.GetIPProperties().UnicastAddresses
                        .Where(y => y.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        .Select(y => new NetworkInterfaceItemModel { IP4Address = y.Address.ToString(), SubnetMask = y.IPv4Mask.ToString() }).ToArray();
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public NetworkInterfaceItemModel[] Addresses { get; set; }
    }
}
