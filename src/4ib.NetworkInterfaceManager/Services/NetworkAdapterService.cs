using _4ib.NetworkInterfaceManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace _4ib.NetworkInterfaceManager.Services
{
    public class NetworkAdapterService
    {
        public static NetworkInterfaceModel[] GetAllModelsIPv4()
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(x => x.Supports(NetworkInterfaceComponent.IPv4))
                .Select(x => new NetworkInterfaceModel(x))
                .ToArray();
        }

        public static NetworkInterfaceModel GetOneModel(string networkInterfaceName)
        {
            return new NetworkInterfaceModel(GetOne(networkInterfaceName));
        }

        public static NetworkInterface GetOne(string networkInterfaceName)
        {
            return NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(x => x.Name == networkInterfaceName);
        }

        public static (string ipAddress, string subnetMask)[] GetListIPv4(NetworkInterface networkInterface)
        {
            return networkInterface.GetIPProperties().UnicastAddresses
                .Where(y => y.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .Select(x => (x.Address.ToString(), x.IPv4Mask.ToString()))
                .ToArray();
        }

        public static bool IsIPv4Exists(NetworkInterface networkInterface, string ipAddress)
        {
            return GetListIPv4(networkInterface).Any(x => x.ipAddress == ipAddress);
        }
    }
}
