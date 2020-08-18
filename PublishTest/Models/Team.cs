using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PublishTest.Models
{
    public class Team
    {
        /// <summary>
        /// 团队ID
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 团队名称
        /// </summary>
        [Display(Name="团队名称")]
        public string Name { get; set; }
    }
}
