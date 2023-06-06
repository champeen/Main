using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Management_of_Change.Models;

namespace Management_of_Change.Data
{
    public class Management_of_ChangeContext : DbContext
    {
        public Management_of_ChangeContext (DbContextOptions<Management_of_ChangeContext> options)
            : base(options)
        {
        }

        public DbSet<Management_of_Change.Models.ChangeRequest> ChangeRequest { get; set; } = default!;

        public DbSet<Management_of_Change.Models.ChangeType>? ChangeType { get; set; }
    }
}
