using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using MicroService.Core.Registry.Extensions;
using MicroService.MemberService.Context;
using MicroService.MemberService.Repository;
using MicroService.MemberService.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Servicecomb.Saga.Omega.AspNetCore.Extensions;

namespace MicroService.MemberService
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
            services.AddDbContext<MemberContext>(options =>  options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            //��ӳ�Ա�ִ�
            services.AddScoped<IMemberRepository, MemberRepository>();
            //��ӳ�Ա����
            services.AddScoped<IMemberService, MemberService.Service.MemberService>();
            //���consulע��
            services.AddConsulRegistry(Configuration);

            //���saga����
            services.AddOmegaCore(options =>
            {
                options.GrpcServerAddress = "localhost:8091";//Э�����ĵ�ַ
                options.InstanceId = "MemberService-1";// 2������ʵ��Id
                options.ServiceName = "MemberService";// 3����������
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

            //ע��Consul
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
