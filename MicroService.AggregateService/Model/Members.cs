﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.AggregateService.Model
{
    public class Members
    {
        /// <summary>
        /// 团队成员主键
        /// </summary>
        public int Id { set; get; }
        /// <summary>
        /// 团队成员名
        /// </summary>
        public string FirstName { set; get; }
        /// <summaryhua
        /// 团队成员花名
        /// </summary>
        public string NickName { set; get; }

        /// <summary>
        /// 团队主键
        /// </summary>
        public int TeamId { set; get; }
    }
}
