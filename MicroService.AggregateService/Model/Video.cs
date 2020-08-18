using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.AggregateService.Model
{
    public class Video
    {
        /// <summary>
        /// 视频主键
        /// </summary>
        public int Id { set; get; }

        /// <summary>
        /// 视频url
        /// </summary>
        public string VideoUrl { set; get; }

        /// <summary>
        /// 成员Id
        /// </summary>
        public int MemberId { set; get; }
    }
}
