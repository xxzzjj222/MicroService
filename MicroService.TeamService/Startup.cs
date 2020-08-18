using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using IdentityServer4.AccessTokenValidation;
using MicroService.Core.Registry.Extensions;
using MicroService.TeamService.Context;
using MicroService.TeamService.Repositories;
using MicroService.TeamService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Servicecomb.Saga.Omega.AspNetCore.Extensions;

namespace MicroService.TeamService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //1.注册上下文到IOC容器
            services.AddDbContext<TeamContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            //2.注册团队仓储
            services.AddScoped<ITeamRepository, TeamRepository>();
            //3.注册团队服务
            services.AddScoped<ITeamService, TeamServiceImpl>();
            //注册consul服务
            services.AddConsulRegistry(Configuration);
            ////注册ids4认证
            //services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            //    .AddIdentityServerAuthentication(options =>
            //    {
            //        options.Authority = "http://localhost:5008/";//1.授权中心地址
            //        options.ApiName = "TeamService";//2.api名称
            //        options.RequireHttpsMetadata = false;//3.https元数据，不需要
            //    });

            // 7、注册saga分布式事务
            services.AddOmegaCore(option =>
            {
                option.GrpcServerAddress = "localhost:8091"; // 1、协调中心地址
                option.InstanceId = "TeamService-1";// 2、服务实例Id
                option.ServiceName = "TeamService";// 3、服务名称
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
            
            app.UseHttpsRedirection();

            //consul服务注册
            app.UseConsulRegistry();

            // 设置允许所有来源跨域
            app.UseCors(options =>
            {
                options.AllowAnyHeader();
                options.AllowAnyMethod();
                options.AllowAnyOrigin();
                //options.AllowCredentials();
            });

            app.UseRouting();

            //app.UseAuthentication();//使用认证
            app.UseAuthorization();//使用授权   

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});
                endpoints.MapControllers();
            });
        }
    }
}
