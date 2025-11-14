using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EHS.Models.IH
{
    public class chemical_risk_assessment : TimeStamps
    {
        [Key]
        public int id { get; set; }

        // Make required strings non-nullable. (No '?' if you truly require them.)
        [Display(Name = "Location")]
        [Required, MaxLength(256)]
        public string location { get; set; } = string.Empty;

        [Display(Name = "Area")]
        // Npgsql maps List<string> to PostgreSQL text[] nicely. Prefer arrays over JSON when it’s “just strings”.
        [Column(TypeName = "text[]")]
        public List<string> area { get; set; } = new();

        [Display(Name = "Use")]
        [Column(TypeName = "text[]")]
        public List<string> use { get; set; } = new();

        [Display(Name = "State")]
        [Required, MaxLength(64)]
        public string state { get; set; } = string.Empty;

        [Display(Name = "Chemical")]
        [Required, MaxLength(256)]
        public string chemical { get; set; } = string.Empty;

        [Display(Name = "NFPA")]
        [MaxLength(128)]
        public string? nfpa { get; set; }

        // Hazards
        [Display(Name = "Inhalation")]
        [Column(TypeName = "text[]")]
        public List<string> inhalation { get; set; } = new();

        [Display(Name = "Skin Contact")]
        [Column(TypeName = "text[]")]
        public List<string> skin_contact { get; set; } = new();

        [Display(Name = "Eye Contact")]
        [Column(TypeName = "text[]")]
        public List<string> eye_contact { get; set; } = new();

        [Display(Name = "Ingestion")]
        [Column(TypeName = "text[]")]
        public List<string> ingestion { get; set; } = new();

        // PPE
        [Display(Name = "Glove")]
        [Column(TypeName = "text[]")]
        public List<string> glove { get; set; } = new();

        [Display(Name = "Suit")]
        [Column(TypeName = "text[]")]
        public List<string> suit { get; set; } = new();

        [Display(Name = "Eyewear")]
        [Column(TypeName = "text[]")]
        public List<string> eyewear { get; set; } = new();

        [Display(Name = "Respiratory")]
        [Column(TypeName = "text[]")]
        public List<string> respiratory { get; set; } = new();

        // If OELs will be structured later, consider a separate table.
        [Display(Name = "OELs")]
        public string? oels { get; set; }

        [Display(Name = "Emergency Response")]
        public string? emergency_response { get; set; }

        [Display(Name = "Notes")]
        public string? notes { get; set; }

        // Computed needs SQL in the migration or fluent config; the annotation alone doesn’t define the expression.
        [Display(Name = "Risk Score")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? risk_score { get; set; }

        [Display(Name = "Person Performing Assessment (Username)")]
        [Required, MaxLength(128)]
        public string person_performing_assessment_username { get; set; } = string.Empty;

        [Display(Name = "Person Performing Assessment")]
        [MaxLength(256)]
        public string? person_performing_assessment_displayname { get; set; }

        [Display(Name = "Date Conducted")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime date_conducted { get; set; }

        [Display(Name = "Review Date")]
        [DataType(DataType.Date)]
        public DateTime? date_reviewed { get; set; }

        // *** One-to-many child list ***
        [Display(Name = "Composition")]
        public ICollection<chemical_composition> composition { get; set; } = new List<chemical_composition>();
    }
}