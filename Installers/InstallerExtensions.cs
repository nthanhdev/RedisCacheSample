using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisCache.Installers
{
    public static class InstallerExtensions
    {
        public static void InstallerServicesInAssembly(this IServiceCollection services , IConfiguration configuration){
            var installers = typeof(Program).Assembly.ExportedTypes.Where(p => typeof(IInstaller).IsAssignableFrom(p) && !p.IsInterface 
                && !p.IsAbstract
            ).Select(Activator.CreateInstance).Cast<IInstaller>().ToList(); 

            installers.ForEach(item => item.InstallServices(services , configuration));
        }
    }
}