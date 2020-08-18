using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.TeamService.Models
{
    public class Team
    {
        /// <summary>
        /// 团队ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 团队名称
        /// </summary>
        public string Name { get; set; }
    }
}
