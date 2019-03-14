using System;
using System.Configuration.Install;
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;

namespace _4ib.NetworkInterfaceManager
{
    static class Program
    {
        static void Main()
        {
            var service = new NetworkInterfaceService();
            var servicesToRun = new ServiceBase[] { service };

            if (Environment.UserInteractive)
            {
                while (true)
                {
                    Console.WriteLine($"///////////////{NetworkInterfaceService.Name}///////////////");
                    Console.WriteLine("/i - install service (running in administration mode is required);");
                    Console.WriteLine("/u - uninstall service (running in administration mode is required);");
                    Console.WriteLine("/d - start service without installation;");
                    Console.WriteLine("////////////////////////////////////////////////////////");
                    switch (Console.ReadLine())
                    {
                        case "/i":
                            ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
                            continue;
                        case "/u":
                            ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
                            continue;
                        case "/d":
                            Console.CancelKeyPress += (x, y) => service.Stop();

                            service.LoggingError = 
                            service.LoggingInfo = s => Console.WriteLine($"{DateTime.Now}: {s}");

                            service.Start();
                            Console.WriteLine("Press any button to stop the service.");
                            Console.ReadKey();
                            service.Stop();
                            continue;
                        default:
                            Console.WriteLine("Unknown command, try again.");
                            continue;
                    }
                }
            }
            else
            {
                service.LoggingInfo = s => service.EventLog.WriteEntry(s, EventLogEntryType.Information);
                service.LoggingError = s => service.EventLog.WriteEntry(s, EventLogEntryType.Error);
            }

            ServiceBase.Run(servicesToRun);
        }
    }
}
