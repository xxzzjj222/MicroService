using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroService.Core.Registry.Extensions
{
    public static class MicroServiceConsulApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseConsulRegistry(this IApplicationBuilder app)
        {
            //1.从IOC容器中获取Consul服务注册配置
            var serviceNode = app.ApplicationServices.GetRequiredService<IOptions<ServiceRegistryConfig>>().Value;

            //2.获取应用生命周期
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            //3.获取服务注册实例
            var serviceRegistry = app.ApplicationServices.GetRequiredService<IServiceRegistry>();

            //4.获取服务地址
            //4.1Kestrel服务 可用动态获取服务地址
            //var features = app.Properties["server.Features"] as FeatureCollection;
            //var address = features.Get<IServerAddressesFeature>().Addresses.First();
            //var uri = new Uri(address);
            //4.2代理服务 通过配置获取

            //5.注册服务
            serviceNode.Id = Guid.NewGuid().ToString();
            //serviceNode.Address=uri.Host;
            //serviceNode.Port = uri.Port;
            //serviceNode.HealthCheckAddress= $"{uri.Scheme}://{uri.Host}:{uri.Port}{serviceNode.HealthCheckAddress}";
            serviceNode.HealthCheckAddress = $"http://{serviceNode.Address}:{serviceNode.Port}{serviceNode.HealthCheckAddress}";
            serviceRegistry.Register(serviceNode);

            //6.服务器关闭时注销服务
            lifetime.ApplicationStopping.Register(() => serviceRegistry.Deregister(serviceNode));

            return app;
        }
    }
}
