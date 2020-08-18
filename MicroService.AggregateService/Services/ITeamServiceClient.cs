﻿using MicroService.AggregateService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.AggregateService.Services
{
    public interface ITeamServiceClient
    {
        /// <summary>
        /// 服务调用
        /// </summary>
        /// <returns></returns>
        Task<IList<Teams>> GetTeams();

        /// <summary>
        /// 添加团队信息
        /// </summary>
        /// <param name="team"></param>
        void InsertTeams(Teams team);
    }
}
