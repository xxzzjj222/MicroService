using MicroService.Core.Registry;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroService.Core.Cluster
{
    public interface ILoadBalance
    {
        /// <summary>
        /// 服务选择
        /// </summary>
        /// <param name="serviceUrls"></param>
        /// <returns></returns>
        ServiceUrl Select(IList<ServiceUrl> serviceUrls);
    }
}
