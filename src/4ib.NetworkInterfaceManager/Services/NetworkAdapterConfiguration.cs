using _4ib.NetworkInterfaceManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace _4ib.NetworkInterfaceManager.Services
{
    public class NetworkAdapterConfiguration
    {
        public static readonly Dictionary<uint, (string message, bool success)> ReturnValues = new Dictionary<uint, (string message, bool success)>
        {
            { 0,  ("Successful completion, no reboot required", true) },
            { 1,  ("Successful completion, reboot required", true) },
            { 66, ("Invalid subnet mask", false) },
            { 70, ("Invalid IP address", false) }
        };

        private NetworkInterface _networkInterface;
        private ManagementClass _networkAdapterConfigurationClass;

        public NetworkAdapterConfiguration(NetworkInterface networkInterface)
        {
            _networkAdapterConfigurationClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
            _networkInterface = networkInterface;
        }

        public (string message, bool success) AddStaticIPv4(IPAddress ipAddress, IPAddress subnetMask)
        {
            ipAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
            subnetMask = subnetMask ??  throw new ArgumentNullException(nameof(subnetMask));

            var networkAdapterObject = _networkAdapterConfigurationClass
                .GetInstances().Cast<ManagementObject>()
                .Where(x => (bool)x["IPEnabled"] == true && x["Description"].Equals(_networkInterface.Description)).FirstOrDefault();

            var enableStaticIPMethod = networkAdapterObject.GetMethodParameters("EnableStatic");

            var currentIpAddresses = NetworkAdapterService.GetListIPv4(_networkInterface);

            var ipAddresses = currentIpAddresses
                .Select(x => x.ipAddress)
                .Concat(new string[] { ipAddress.ToString() })
                .ToArray();

            var subnets = currentIpAddresses
                .Select(x => x.subnetMask)
                .Concat(new string[] { subnetMask.ToString() })
                .ToArray();

            enableStaticIPMethod["IPAddress"] = ipAddresses;
            enableStaticIPMethod["SubnetMask"] = subnets;

            var setIPMethod = networkAdapterObject.InvokeMethod("EnableStatic", enableStaticIPMethod, null);

            uint result = (uint)setIPMethod["ReturnValue"];

            return ReturnValues.ContainsKey(result) ? ReturnValues[result] : ($"Unknown problem, result #{result}", false);
        }
    }
}
