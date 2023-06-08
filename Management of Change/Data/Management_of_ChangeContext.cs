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

        public DbSet<Management_of_Change.Models.ChangeLevel>? ChangeLevel { get; set; }

        public DbSet<Management_of_Change.Models.ChangeStep>? ChangeStep { get; set; }

        public DbSet<Management_of_Change.Models.ResponseDropdownSelections>? ResponseDropdownSelections { get; set; }

        public DbSet<Management_of_Change.Models.ProductLine>? ProductLine { get; set; }

        public DbSet<Management_of_Change.Models.SiteLocation>? SiteLocation { get; set; }

        public DbSet<Management_of_Change.Models.ChangeArea>? ChangeArea { get; set; }

        public DbSet<Management_of_Change.Models.GeneralMocQuestions>? GeneralMocQuestions { get; set; }
    }
}
