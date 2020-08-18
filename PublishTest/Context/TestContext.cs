using Microsoft.EntityFrameworkCore;
using PublishTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishTest.Context
{
    public class TestContext : DbContext
    {
        public TestContext(DbContextOptions options)
            : base(options)
        {
            
        }

        public DbSet<Team> Teams { get; set; }
    }
}
