using System;
using System.Collections.Generic;
using System.Text;

namespace MicroService.Core.Registry
{
    public interface IServiceRegistry
    {
        //注册服务
        void Register(ServiceRegistryConfig serviceNode);
        //撤销服务
        void Deregister(ServiceRegistryConfig serviceNode);
    }
}
