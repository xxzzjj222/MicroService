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

            // 1、自定义异常处理(用缓存处理)
            var fallbackResponse = new HttpResponseMessage
            {
                Content = new StringContent("系统正繁忙，请稍后重试"),// 内容，自定义内容
                StatusCode = HttpStatusCode.GatewayTimeout // 504
            };
            //注册HttpClient Polly
            services.AddPollyHttpClient("micro", options =>
             {
                options.TimeoutTime = 3; // 1、超时时间
                options.RetryCount = 3;// 2、重试次数
                options.CircuitBreakerOpenFallCount = 1;// 3、熔断器开启(多少次失败开启)
                options.CircuitBreakerDownTime = 100;// 4、熔断器开启时间
                options.httpResponseMessage = fallbackResponse;// 5、降级处理
            });
            //添加HttpClient Consul发现
            services.AddHttpClientConsul<ConsulHttpClient>();
            
            services.AddScoped<ITeamServiceClient, HttpTeamService>();

            services.AddScoped<IMemberServiceClient, HttpMemberServiceClient>();

            //添加consul注册
            services.AddConsulRegistry(Configuration);

            //添加saga事务
            services.AddOmegaCore(options =>
            {
                options.GrpcServerAddress = "localhost:8091";//协调中心地址
                options.InstanceId = "AggregateService-1";// 2、服务实例Id
                options.ServiceName = "AggregateService";// 3、服务名称
            });

            services.AddCap(options =>
            {
                //options.UseInMemoryStorage();

                // 1 使用EntityFramework进行存储操作
                options.UseEntityFramework<AggregateContext>();

                //2.使用sqlserver进行事务处理
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

                //rabbiimq进行事件中心处理
                options.UseRabbitMQ(config =>
                {
                    config.HostName = "localhost";
                    config.UserName = "guest";
                    config.Password = "guest";
                    config.Port = 5672;
                    config.VirtualHost = "/";
                });

                //添加cap后台监控页面
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
            //注册到consul
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
