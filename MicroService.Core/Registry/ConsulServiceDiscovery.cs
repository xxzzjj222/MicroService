using Consul;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MicroService.Core.Registry
{
    public class ConsulServiceDiscovery : IServiceDiscovery
    {
        private readonly IConfiguration configuration;
        public ConsulServiceDiscovery(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<IList<ServiceUrl>> Discovery(string serviceName)
        {
            ServiceDiscoveryConfig serviceDiscoveryConfig = configuration.GetSection("ConsulDiscovery").Get<ServiceDiscoveryConfig>();

            //创建consul客户端连接
            var consulClient = new ConsulClient(config => config.Address = new Uri(serviceDiscoveryConfig.RegistryAddress));
            //查询服务
            var queryResult =await consulClient.Catalog.Service(serviceName);
            //拼接服务
            var list = new List<ServiceUrl>();
            foreach (var service in queryResult.Response)
            {
                list.Add(new ServiceUrl { Url = service.ServiceAddress + ":" + service.ServicePort });
            }
            return list;
        }
    }
}
