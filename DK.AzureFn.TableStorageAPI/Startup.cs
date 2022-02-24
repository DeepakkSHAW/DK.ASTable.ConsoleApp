using DK.AzureFn.TableStorageAPI;
using Microsoft.Extensions.Configuration;
using DK.AzureTableStorage.Operations.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

[assembly: FunctionsStartup(typeof(Startup))]

namespace DK.AzureFn.TableStorageAPI
{

    public class Startup : FunctionsStartup
    {
        private readonly string _ConfigFilePath = "appsettings.json";
        //override ConfigurationBuilder Settings
        //https://adamstorr.azurewebsites.net/blog/why-wont-you-load-my-configuration-azure-functions
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            //Overrideden behavior read from, appsettings.json
            FunctionsHostBuilderContext context = builder.GetContext();
            
            builder.ConfigurationBuilder
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, _ConfigFilePath), optional: false, reloadOnChange: true)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.{context.EnvironmentName}.json"), optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();

            //Default behavior read from local.settings.json
            //base.ConfigureAppConfiguration(builder);
        }
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddTransient<IAzureTableHandler, AzureTableHandler>();
        }
    }
}
