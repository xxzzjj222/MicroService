using Consul;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroService.Core.Registry
{
    public class ConsulServiceRegistry : IServiceRegistry
    {
        public void Register(ServiceRegistryConfig serviceNode)
        {
            //创建consul客户端
            var consulClient = new ConsulClient(config => config.Address = new Uri(serviceNode.RegistryAddress));
            //创建consul服务注册对象
            var registration = new AgentServiceRegistration()
            {
                ID=serviceNode.Id,
                Name=serviceNode.Name,
                Address=serviceNode.Address,
                Port=serviceNode.Port,
                Tags=serviceNode.Tags,
                Check=new AgentServiceCheck()
                {
                    //健康检查超时时间
                    Timeout=TimeSpan.FromSeconds(10),
                    //服务停止5秒后注销服务
                    DeregisterCriticalServiceAfter=TimeSpan.FromSeconds(5),
                    //健康检查地址
                    HTTP=serviceNode.HealthCheckAddress,
                    //健康检查时间间隔
                    Interval=TimeSpan.FromSeconds(10)
                }
            };
            //注册服务
            consulClient.Agent.ServiceRegister(registration);
            //关闭连接
            consulClient.Dispose();
        }

        public void Deregister(ServiceRegistryConfig serviceNode)
        {
            //创建客户端连接
            var consulClient = new ConsulClient(config => config.Address = new Uri(serviceNode.RegistryAddress));
            //注销服务
            consulClient.Agent.ServiceDeregister(serviceNode.Id);
            //关闭连接
            consulClient.Dispose();
        }
    }
}
