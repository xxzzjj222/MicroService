using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.AggregateService.Context
{
    public class AggregateContext:DbContext
    {
        public AggregateContext(DbContextOptions<AggregateContext> options)
            :base(options)
        {

        }
    }
}
