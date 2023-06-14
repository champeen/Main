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

        public DbSet<Management_of_Change.Models.ChangeStatus>? ChangeStatus { get; set; }

        public DbSet<Management_of_Change.Models.ResponseDropdownSelections>? ResponseDropdownSelections { get; set; }

        public DbSet<Management_of_Change.Models.ProductLine>? ProductLine { get; set; }

        public DbSet<Management_of_Change.Models.SiteLocation>? SiteLocation { get; set; }

        public DbSet<Management_of_Change.Models.ChangeArea>? ChangeArea { get; set; }

        public DbSet<Management_of_Change.Models.GeneralMocQuestions>? GeneralMocQuestions { get; set; }

        public DbSet<Management_of_Change.Models.GeneralMocResponses>? GeneralMocResponses { get; set; }

        public DbSet<Management_of_Change.Models.ReviewType>? ReviewType { get; set; }

        public DbSet<Management_of_Change.Models.ImpactAssessmentMatrix>? ImpactAssessmentMatrix { get; set; }

        public DbSet<Management_of_Change.Models.ImpactAssessmentResponse>? ImpactAssessmentResponse { get; set; }

        public DbSet<Management_of_Change.Models.FinalReviewType>? FinalReviewType { get; set; }
    }
}
