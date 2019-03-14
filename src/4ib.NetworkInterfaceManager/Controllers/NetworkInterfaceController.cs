using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Web.Http;
using System.Web.Http.Results;
using _4ib.NetworkInterfaceManager.Models;
using _4ib.NetworkInterfaceManager.Services;

namespace _4ib.NetworkInterfaceManager.Controllers
{
    [RoutePrefix("api")]
    public class NetworkInterfaceController : ApiController
    {
        [HttpGet]
        [Route("network/interface")]
        public IEnumerable<NetworkInterfaceModel> GetNetworkInterfaceList()
        {
            return NetworkAdapterService.GetAllModelsIPv4();
        }

        [HttpPost]
        [Route("network/interface")]
        public IHttpActionResult AddNetworkInterface(string networkInterfaceName, string ipAddress, string subnetMask)
        {
            // Validate params

            if (string.IsNullOrWhiteSpace(ipAddress) || !IPAddress.TryParse(ipAddress, out IPAddress ipAddressObj))
                return BadRequest($"Unknown format of {nameof(ipAddress)}");

            if (string.IsNullOrWhiteSpace(subnetMask) || !IPAddress.TryParse(subnetMask, out IPAddress subnetMaskObj))
                return BadRequest($"Unknown format of {nameof(subnetMask)}");

            var ipAddressNormalized = ipAddressObj.ToString();

            if (string.IsNullOrWhiteSpace(networkInterfaceName))
                return BadRequest($"{nameof(networkInterfaceName)} is empty");

            // Load network interface

            var networkInterface = NetworkAdapterService.GetOne(networkInterfaceName);

            if (networkInterface == null)
                return new ResponseMessageResult(Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Such Network Interface '{networkInterfaceName}' not found"));

            if (string.IsNullOrWhiteSpace(networkInterface.Description))
                return new ResponseMessageResult(Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Network Interface cannot be found if Description missed"));

            if (NetworkAdapterService.IsIPv4Exists(networkInterface, ipAddressNormalized))
                return new ResponseMessageResult(Request.CreateErrorResponse(HttpStatusCode.Found, $"This IP Address '{ipAddressNormalized}' already added"));

            // Try adding IPv4

            var (message, success) = new NetworkAdapterConfiguration(networkInterface).AddStaticIPv4(ipAddressObj, subnetMaskObj);

            if (success)
            {
                return Ok(NetworkAdapterService.GetOneModel(networkInterface.Name));
            }
            else
            {
                return new ResponseMessageResult(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, message));
            }
        }
    }
}
