using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Winton.Extensions.Configuration.Consul;

namespace MicroService.TeamService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddCommandLine(args)//支持命令行参数
                .Build();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //动态加载配置中心的配置文件
                    //webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                    //{
                    //    //加载默认的配置到Configuration
                    //    hostingContext.Configuration = config.Build();

                    //    //加载consul配置中心
                    //    string consul_url = hostingContext.Configuration["Consul_Url"];
                    //    var env = hostingContext.HostingEnvironment;
                    //    config.AddConsul($"{env.ApplicationName}/appsettings.json", options =>
                    //     {
                    //         options.ConsulConfigurationOptions = cco => cco.Address = new Uri(consul_url);
                    //         options.Optional = true;
                    //         options.ReloadOnChange = true;
                    //         options.OnLoadException = exception => exception.Ignore = true;
                    //     });

                    //    //动态加载环境信息，主要在于动态获取服务名称和环境名称


                    //    //conusl中加载的配置信息加载到Configuration对象，然后通过Configuration对象加载到项目中
                    //    hostingContext.Configuration = config.Build();

                    //});

                    webBuilder.UseStartup<Startup>();
                });
    }
}
