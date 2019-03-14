using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace _4ib.NetworkInterfaceManager.Service
{
    [RunInstaller(true)]
    public class NetworkInterfaceInstaller : System.Configuration.Install.Installer
    {
        public NetworkInterfaceInstaller()
        {
            Installers.Add(new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem
            });

            Installers.Add(new ServiceInstaller
            {
                StartType = ServiceStartMode.Automatic,
                ServiceName = NetworkInterfaceService.Name
            });
        }

        protected override void OnCommitted(System.Collections.IDictionary savedState)
        {
            new ServiceController(NetworkInterfaceService.Name).Start();
        }
    }
}
