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
            // 1��ע�������ĵ�IOC����
            services.AddDbContext<VideoContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            //2.ע��service
            services.AddScoped<IVideoService, VideoServiceImpl>();
            //3.ע��ִ�
            services.AddScoped<IVideoRepository, VideoRepository>();

            // 4�����ӳ��
            //services.AddAutoMapper();

            //5����ӷ���ע������
            services.AddConsulRegistry(Configuration);

            //6.����¼�����cap
            services.AddCap(x =>
            {
                // 1 ʹ���ڴ�洢��Ϣ(��Ϣ����ʧ�ܴ���)
                //x.UseInMemoryStorage();

                // 1 ʹ��EntityFramework���д洢����
                x.UseEntityFramework<VideoContext>();

                //2.ʹ��sqlserver����������
                x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

                //3.ʹ��rabbitMQ���н����¼����Ĵ���
                x.UseRabbitMQ(rb =>
                {
                    rb.HostName = "localhost";
                    rb.UserName = "guest";
                    rb.Password = "guest";
                    rb.Port = 5672;
                    rb.VirtualHost = "/";
                });

                //4.���ö�ʱ����������
                //x.FailedRetryInterval = 2;
                x.FailedRetryCount = 5; // 3 ��ʧ�� 3����

                //5.�˹���Ԥ���޸ı��������ҳ��
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

            //consul����ע��
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
