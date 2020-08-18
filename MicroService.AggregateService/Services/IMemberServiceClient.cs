using MicroService.AggregateService.Model;
using MicroService.Core.HttpClientConsul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.AggregateService.Services
{
    public interface IMemberServiceClient
    {
        Task<IList<Members>> GetMembers(int teamId);

        /// <summary>
        /// 添加团队成员信息
        /// </summary>
        /// <param name="member"></param>
        void InsertMembers(Members member);
    }
}
