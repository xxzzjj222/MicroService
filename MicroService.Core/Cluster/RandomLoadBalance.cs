using MicroService.Core.Registry;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroService.Core.Cluster
{
    public class RandomLoadBalance:AbstractLoadBalance
    {
        private readonly Random random = new Random();
        /// <summary>
        /// 随机
        /// </summary>
        /// <param name="serviceUrls"></param>
        /// <returns></returns>
        public override ServiceUrl DoSelect(IList<ServiceUrl> serviceUrls)
        {
            var index = random.Next(serviceUrls.Count);
            return serviceUrls[index];
        }
    }
}
