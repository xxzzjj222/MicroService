using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroService.Core.Registry.Extensions
{
    public static class MicroServiceConsulServiceCollectionExtensions
    {
        /// <summary>
        /// 服务注册
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddConsulRegistry(this IServiceCollection services, IConfiguration configuration)
        {
            //加载consul服务注册配置
            services.Configure<ServiceRegistryConfig>(configuration.GetSection("ConsulRegistry"));
            //注册consul服务
            services.AddSingleton<IServiceRegistry, ConsulServiceRegistry>();
            return services;
        }
        /// <summary>
        /// 服务发现
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddConsulDiscovery(this IServiceCollection services)
        {
            //注册服务发现
            services.AddSingleton<IServiceDiscovery, ConsulServiceDiscovery>();
            return services;
        }
    }
}
