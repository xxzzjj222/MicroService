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
                .AddCommandLine(args)//֧�������в���
                .Build();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //��̬�����������ĵ������ļ�
                    //webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                    //{
                    //    //����Ĭ�ϵ����õ�Configuration
                    //    hostingContext.Configuration = config.Build();

                    //    //����consul��������
                    //    string consul_url = hostingContext.Configuration["Consul_Url"];
                    //    var env = hostingContext.HostingEnvironment;
                    //    config.AddConsul($"{env.ApplicationName}/appsettings.json", options =>
                    //     {
                    //         options.ConsulConfigurationOptions = cco => cco.Address = new Uri(consul_url);
                    //         options.Optional = true;
                    //         options.ReloadOnChange = true;
                    //         options.OnLoadException = exception => exception.Ignore = true;
                    //     });

                    //    //��̬���ػ�����Ϣ����Ҫ���ڶ�̬��ȡ�������ƺͻ�������


                    //    //conusl�м��ص�������Ϣ���ص�Configuration����Ȼ��ͨ��Configuration������ص���Ŀ��
                    //    hostingContext.Configuration = config.Build();

                    //});

                    webBuilder.UseStartup<Startup>();
                });
    }
}
