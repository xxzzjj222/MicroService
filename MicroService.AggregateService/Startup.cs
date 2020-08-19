using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using MicroService.AggregateService.Context;
using MicroService.AggregateService.Services;
using MicroService.Core.HttpClientConsul;
using MicroService.Core.HttpClientPolly;
using MicroService.Core.Registry.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Servicecomb.Saga.Omega.AspNetCore.Extensions;

namespace MicroService.AggregateService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AggregateContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            // 1���Զ����쳣����(�û��洦��)
            var fallbackResponse = new HttpResponseMessage
            {
                Content = new StringContent("ϵͳ����æ�����Ժ�����"),// ���ݣ��Զ�������
                StatusCode = HttpStatusCode.GatewayTimeout // 504
            };
            //ע��HttpClient Polly
            services.AddPollyHttpClient("micro", options =>
             {
                options.TimeoutTime = 3; // 1����ʱʱ��
                options.RetryCount = 3;// 2�����Դ���
                options.CircuitBreakerOpenFallCount = 1;// 3���۶�������(���ٴ�ʧ�ܿ���)
                options.CircuitBreakerDownTime = 100;// 4���۶�������ʱ��
                options.httpResponseMessage = fallbackResponse;// 5����������
            });
            //���HttpClient Consul����
            services.AddHttpClientConsul<ConsulHttpClient>();
            
            services.AddScoped<ITeamServiceClient, HttpTeamService>();

            services.AddScoped<IMemberServiceClient, HttpMemberServiceClient>();

            //���consulע��
            services.AddConsulRegistry(Configuration);

            //���saga����
            services.AddOmegaCore(options =>
            {
                options.GrpcServerAddress = "localhost:8091";//Э�����ĵ�ַ
                options.InstanceId = "AggregateService-1";// 2������ʵ��Id
                options.ServiceName = "AggregateService";// 3����������
            });

            services.AddCap(options =>
            {
                //options.UseInMemoryStorage();

                // 1 ʹ��EntityFramework���д洢����
                options.UseEntityFramework<AggregateContext>();

                //2.ʹ��sqlserver����������
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

                //rabbiimq�����¼����Ĵ���
                options.UseRabbitMQ(config =>
                {
                    config.HostName = "localhost";
                    config.UserName = "guest";
                    config.Password = "guest";
                    config.Port = 5672;
                    config.VirtualHost = "/";
                });

                //���cap��̨���ҳ��
                options.UseDashboard();
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //ע�ᵽconsul
            app.UseConsulRegistry();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
