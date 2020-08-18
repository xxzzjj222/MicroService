using MicroService.Core.Registry;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroService.Core.Cluster
{
    public abstract class AbstractLoadBalance : ILoadBalance
    {
        public ServiceUrl Select(IList<ServiceUrl> serviceUrls)
        {
            if (serviceUrls == null || serviceUrls.Count == 0)
            {
                return null;
            }
            if (serviceUrls.Count == 1)
            {
                return serviceUrls[0];
            }
            return DoSelect(serviceUrls);
        }
        /// <summary>
        /// 子类扩展
        /// </summary>
        /// <param name="serviceUrls"></param>
        /// <returns></returns>
        public abstract ServiceUrl DoSelect(IList<ServiceUrl> serviceUrls);
    }
}
