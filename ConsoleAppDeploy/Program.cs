using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using System;

namespace ConsoleAppDeploy
{
    /// <summary>
    /// Call Example
    /// 
    ///     dotnet consoleappdeploy.dll -d testdevice -f "C:\...\config\deployment.amd64.json" -i "connection_string_iot_hub"
    /// 
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "IoT-Edge Deploy",
                Description = "App that let you deploy an IoT-Edge Deployment"
            };

            app.HelpOption("-?|-h|--help");

            var deviceNameOpt = app.Option("-d|--devicename <device-name>", "Target Device on which you want to apply the deploy", CommandOptionType.SingleValue);
            var deploymentFileOpt = app.Option("-f|--file", "File containing the deployment to apply", CommandOptionType.SingleValue);
            var iotHubConnectionStringOpt = app.Option("-i|--iothub", "IoT Hub connection string", CommandOptionType.SingleValue);

            app.OnExecute(async () =>
            {
                if (!deviceNameOpt.HasValue() || !deploymentFileOpt.HasValue() || !iotHubConnectionStringOpt.HasValue())
                    app.ShowHint();
                else
                {
                    var deployment = File.ReadAllText(deploymentFileOpt.Value());
                    var hub = RegistryManager.CreateFromConnectionString(iotHubConnectionStringOpt.Value());
                    await hub.ApplyConfigurationContentOnDeviceAsync(deviceNameOpt.Value(), JsonConvert.DeserializeObject<ConfigurationContent>(deployment));
                }
                return 0;
            });

            app.Execute(args);
        }

        /*
          if your app had more than one command each with their own parameters as in: docker build, run, volume, inspect, ...
          you could add command as follow:

                app.Command("run", (command) =>
                {
                    command.Description = "Let you run a docker image";
                    command.HelpOption("-?|-h|--help");
                    command.Option("--name", "name of the docker image to run as container", CommandOptionType.SingleValue);

                    command.OnExecute(() =>
                    {
                        //same flavour as app.OnExecute
                        return 0;
                    });
                });          
        
        */
    }
}
