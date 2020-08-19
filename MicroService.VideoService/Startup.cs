using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroService.Core.Registry.Extensions;
using MicroService.VideoService.Context;
using MicroService.VideoService.Repositories;
using MicroService.VideoService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MicroService.VideoService
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
            // 1、注册上下文到IOC容器
            services.AddDbContext<VideoContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            //2.注册service
            services.AddScoped<IVideoService, VideoServiceImpl>();
            //3.注册仓储
            services.AddScoped<IVideoRepository, VideoRepository>();

            // 4、添加映射
            //services.AddAutoMapper();

            //5、添加服务注册条件
            services.AddConsulRegistry(Configuration);

            //6.添加事件总线cap
            services.AddCap(x =>
            {
                // 1 使用内存存储消息(消息发送失败处理)
                //x.UseInMemoryStorage();

                // 1 使用EntityFramework进行存储操作
                x.UseEntityFramework<VideoContext>();

                //2.使用sqlserver进行事务处理
                x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

                //3.使用rabbitMQ进行进行事件中心处理
                x.UseRabbitMQ(rb =>
                {
                    rb.HostName = "localhost";
                    rb.UserName = "guest";
                    rb.Password = "guest";
                    rb.Port = 5672;
                    rb.VirtualHost = "/";
                });

                //4.配置定时器尽早启动
                //x.FailedRetryInterval = 2;
                x.FailedRetryCount = 5; // 3 次失败 3分钟

                //5.人工干预，修改表，后面管理页面
                x.UseDashboard();

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

            //consul服务注册
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
