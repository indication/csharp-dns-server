
namespace Dns
{
    using Ninject.Modules;  
    using ZoneProvider.AP;

    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<Contracts.IDnsCache>().To<DnsCache>();
            Bind<Contracts.IDnsResolver>().To<SmartZoneResolver>();
            Bind<ZoneProvider.BaseZoneProvider>().ToConstant(new APZoneProvider("data\\machineinfo.csv", ".foo.bar"));
        }
    }
}
