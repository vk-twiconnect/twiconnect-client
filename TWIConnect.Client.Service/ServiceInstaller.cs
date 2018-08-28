using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace TWIConnect.Client.Service
{
    [RunInstaller(true)]
    public sealed class ServiceProcessInstaller : System.ServiceProcess.ServiceProcessInstaller
    {
        public ServiceProcessInstaller()
        {
            this.Account = ServiceAccount.NetworkService;
        }
    }

    [RunInstaller(true)]
    public partial class ServiceInstaller : System.ServiceProcess.ServiceInstaller
    {
        private const string _serviceName = "TWIConnectV2";

        public ServiceInstaller()
        {
            InitializeComponent();
            this.Description = "Transactional Web unattended backup client";
            this.DisplayName = _serviceName;
            this.ServiceName = _serviceName;
            this.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
        }

        internal static void Install(string[] args)
        {
            using (AssemblyInstaller inst = new AssemblyInstaller(typeof(Program).Assembly, args))
            {
                IDictionary state = new Hashtable();
                inst.UseNewContext = true;
                try
                {
                    inst.Install(state);
                    inst.Commit(state);
                    ServiceInstaller.StartService();
                }
                catch (Exception ex)
                {
                    inst.Rollback(state);
                    throw ex;
                }
            }
        }

        internal static void Uninstall(string[] args)
        {
            ServiceInstaller.StopService();
            using (AssemblyInstaller inst = new AssemblyInstaller(typeof(Program).Assembly, args))
            {
                IDictionary state = new Hashtable();
                inst.UseNewContext = true;
                try
                {
                    inst.Uninstall(state);
                }
                catch (Exception ex)
                {
                    inst.Rollback(state);
                    throw ex;
                }
            }
        }

        internal static void StartService()
        {
            ServiceController service = new ServiceController();
            service.ServiceName = _serviceName;

            if (service.Status != ServiceControllerStatus.Running)
            {
                service.Start();
            }
        }

        internal static void StopService()
        {
            ServiceController service = new ServiceController();
            service.ServiceName = _serviceName;

            if (service.Status != ServiceControllerStatus.Stopped)
            {
                service.Stop();
            }
        }
    }
}
