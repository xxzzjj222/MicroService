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
            //添加成员仓储
            services.AddScoped<IMemberRepository, MemberRepository>();
            //添加成员服务
            services.AddScoped<IMemberService, MemberService.Service.MemberService>();
            //添加consul注册
            services.AddConsulRegistry(Configuration);

            //添加saga事务
            services.AddOmegaCore(options =>
            {
                options.GrpcServerAddress = "localhost:8091";//协调中心地址
                options.InstanceId = "MemberService-1";// 2、服务实例Id
                options.ServiceName = "MemberService";// 3、服务名称
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

            //注册Consul
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
