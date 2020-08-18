using MicroService.Core.Cluster;
using MicroService.Core.Registry.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroService.Core.HttpClientConsul
{
    public static class ConsulHttpClientServiceCollectionExtensions
    {
        /// <summary>
        /// 注入
        /// </summary>
        /// <typeparam name="ConsulHttpClient"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpClientConsul<ConsulHttpClient>(this IServiceCollection services) where ConsulHttpClient : class
        {
            //注册consul发现
            services.AddConsulDiscovery();
            //注册服务负载均衡
            services.AddSingleton<ILoadBalance, RandomLoadBalance>();
            //注册consulHttpClient
            services.AddSingleton<ConsulHttpClient>();
            return services;
        }
    }
}
