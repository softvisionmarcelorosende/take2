using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace src
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var directory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    var settings = config.Build();
                    //config.AddJsonFile(Path.Combine(directory,"azureappconfig.json" ));
                    config.AddAzureAppConfiguration(options => options.Connect(settings["ConnectionStrings:AppConfig"])
                       .Watch("Take2:Settings:BackgroundColor", new TimeSpan(0,0,1))
                       .Watch("Take2:Settings:FontColor", new TimeSpan(0,0,1))
                       .Watch("Take2:Settings:FontSize", new TimeSpan(0, 0, 1))
                       .Watch("Take2:Settings:Message", new TimeSpan(0, 0, 1)));
                })
                .UseStartup<Startup>();
    }
}
