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
            ////添加ids4
            //services.AddIdentityServer()
            //    .AddDeveloperSigningCredential()//1.用户登录配置
            //    .AddInMemoryApiResources(Config.GetApiResources())//2.存储api资源
            //    .AddInMemoryClients(Config.GetClients())//3.存储客户端
            //    .AddTestUsers(Config.GetUsers())//4.测试用户
            //    .AddInMemoryIdentityResources(Config.Ids);//5. openId

            // 将Config配置的数据持久化到Sqlserver
            // 资源客户端持久化操作
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
            //用户相关的配置
            services.AddDbContext<IdentityServerUserDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            //添加用户
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                //密码复杂度配置
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
            //初始化配置数据
            InitializeDatabase(app);
            //初始化用户数据
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
            //添加ids4
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
        /// 存储config中数据
        /// </summary>
        /// <param name="app"></param>
        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope=app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                //添加clients
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    //context.SaveChanges();
                }

                //添加api资源
                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    //context.SaveChanges();
                }

                //添加openId
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
        /// 存储用户数据
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
