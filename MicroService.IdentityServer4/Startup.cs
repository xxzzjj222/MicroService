using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using MicroService.IdentityServer4.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MicroService.IdentityServer4
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
            ////���ids4
            //services.AddIdentityServer()
            //    .AddDeveloperSigningCredential()//1.�û���¼����
            //    .AddInMemoryApiResources(Config.GetApiResources())//2.�洢api��Դ
            //    .AddInMemoryClients(Config.GetClients())//3.�洢�ͻ���
            //    .AddTestUsers(Config.GetUsers())//4.�����û�
            //    .AddInMemoryIdentityResources(Config.Ids);//5. openId

            // ��Config���õ����ݳ־û���Sqlserver
            // ��Դ�ͻ��˳־û�����
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddIdentityServer()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                      {
                          builder.UseSqlServer(connectionString, options =>
                          {
                              options.MigrationsAssembly(migrationsAssembly);
                          });
                      };
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                      {
                          builder.UseSqlServer(connectionString, options =>
                           {
                               options.MigrationsAssembly(migrationsAssembly);
                           });
                      };
                })
                .AddDeveloperSigningCredential();
            //�û���ص�����
            services.AddDbContext<IdentityServerUserDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            //����û�
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                //���븴�Ӷ�����
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<IdentityServerUserDbContext>()
                .AddDefaultTokenProviders();

            services.AddControllersWithViews();     
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //��ʼ����������
            InitializeDatabase(app);
            //��ʼ���û�����
            InitializeUserDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            //���ids4
            app.UseIdentityServer();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        /// <summary>
        /// �洢config������
        /// </summary>
        /// <param name="app"></param>
        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope=app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                //���clients
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    //context.SaveChanges();
                }

                //���api��Դ
                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    //context.SaveChanges();
                }

                //���openId
                if (!context.IdentityResources.Any())
                {
                    foreach (var  ids in Config.Ids)
                    {
                        context.IdentityResources.Add(ids.ToEntity());
                    }
                }
                context.SaveChanges();
            }
        }
        /// <summary>
        /// �洢�û�����
        /// </summary>
        /// <param name="app"></param>
        private void InitializeUserDatabase(IApplicationBuilder app)
        {
            using (var serviceScope=app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<IdentityServerUserDbContext>();
                context.Database.Migrate();

                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var identityUser = userManager.FindByNameAsync("xzj").Result;
                if (identityUser==null)
                {
                    identityUser = new IdentityUser
                    {
                        UserName = "xzj",
                        Email = "xzj@email.com"
                    };
                    var result = userManager.CreateAsync(identityUser, "123456").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    result = userManager.AddClaimsAsync(identityUser, new Claim[] {
                        new Claim(JwtClaimTypes.Name, "xzj"),
                        new Claim(JwtClaimTypes.GivenName, "xzj"),
                        new Claim(JwtClaimTypes.FamilyName, "xzj"),
                        new Claim(JwtClaimTypes.Email, "xzj@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://xzj.com")
                    }).Result;

                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                }
            }
        }

    }
}
