using MicroService.TeamService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.TeamService.Context
{
    public class TeamContext:DbContext
    {
        public TeamContext(DbContextOptions<TeamContext> options)
            : base(options)
        { }

        public DbSet<Team> Teams { get; set; }
    }
}
