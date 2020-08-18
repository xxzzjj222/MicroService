using MicroService.VideoService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.VideoService.Context
{
    public class VideoContext:DbContext
    {

        public VideoContext(DbContextOptions options)
            : base(options)
        {
            
        }

        public DbSet<Videos> Videos { get; set; }
    }
}
