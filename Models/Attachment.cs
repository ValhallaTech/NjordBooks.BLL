using System.ComponentModel.DataAnnotations;

namespace NjordBooks.BLL.Models
{
    public class Attachment
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        public Household Household { get; set; }

        [Required]
        [StringLength( 40, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                       MinimumLength = 2 )]
        public string FileName { get; set; }

        [Required]
        [StringLength( 40, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                       MinimumLength = 2 )]
        public string Description { get; set; }

        [Required]
        [StringLength( 40, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                       MinimumLength = 2 )]
        public string ContentType { get; set; }

        public byte[] FileData { get; set; }
    }
}
