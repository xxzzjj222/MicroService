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
            //1.ע�������ĵ�IOC����
            services.AddDbContext<TeamContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            //2.ע���ŶӲִ�
            services.AddScoped<ITeamRepository, TeamRepository>();
            //3.ע���Ŷӷ���
            services.AddScoped<ITeamService, TeamServiceImpl>();
            //ע��consul����
            services.AddConsulRegistry(Configuration);
            ////ע��ids4��֤
            //services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            //    .AddIdentityServerAuthentication(options =>
            //    {
            //        options.Authority = "http://localhost:5008/";//1.��Ȩ���ĵ�ַ
            //        options.ApiName = "TeamService";//2.api����
            //        options.RequireHttpsMetadata = false;//3.httpsԪ���ݣ�����Ҫ
            //    });

            // 7��ע��saga�ֲ�ʽ����
            services.AddOmegaCore(option =>
            {
                option.GrpcServerAddress = "localhost:8091"; // 1��Э�����ĵ�ַ
                option.InstanceId = "TeamService-1";// 2������ʵ��Id
                option.ServiceName = "TeamService";// 3����������
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

            //consul����ע��
            app.UseConsulRegistry();

            // ��������������Դ����
            app.UseCors(options =>
            {
                options.AllowAnyHeader();
                options.AllowAnyMethod();
                options.AllowAnyOrigin();
                //options.AllowCredentials();
            });

            app.UseRouting();

            //app.UseAuthentication();//ʹ����֤
            app.UseAuthorization();//ʹ����Ȩ   

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
