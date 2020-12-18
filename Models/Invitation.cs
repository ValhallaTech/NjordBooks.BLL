using System;
using System.ComponentModel.DataAnnotations;

namespace NjordBooks.BLL.Models
{
    public class Invitation
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        public Household Household { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Expires { get; set; }

        public bool Accepted { get; set; }

        [Required]
        [StringLength( 40, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                       MinimumLength = 2 )]
        public string EmailTo { get; set; }

        [Required]
        [StringLength( 40, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                       MinimumLength = 2 )]
        public string Subject { get; set; }

        [Required]
        [StringLength( 180, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                       MinimumLength = 2 )]
        public string Body { get; set; }

        public Guid Code { get; set; } = Guid.NewGuid( );
    }
}
