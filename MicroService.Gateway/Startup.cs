using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using MicroService.Gateway.IdentityServer;
using MicroService.Gateway.OcelotExtension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Middleware.Pipeline;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using Ocelot.Values;

namespace MicroService.Gateway
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
            //ע��ids4��֤
            var identityServerOptions = new IdentityServerOptions();
            Configuration.Bind("IdentityServerOptions", identityServerOptions);
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(identityServerOptions.IdentityScheme,options =>
                {
                    options.Authority = identityServerOptions.AuthorityAddress; // 1����Ȩ���ĵ�ַ
                    options.ApiName = identityServerOptions.ResourceName; // 2��api����(��Ŀ��������)
                    options.RequireHttpsMetadata = false; // 3��httpsԪ���ݣ�����Ҫ
                });

            services.AddOcelot().AddConsul().AddPolly();
            ///services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //���ocelot����
            app.UseOcelot((build,config)=> {
                build.BuildCustomeOcelotPipeline(config);
            }).Wait();
           

            app.UseRouting();

            app.UseAuthentication();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
