using System;
using System.ComponentModel.DataAnnotations;

namespace NjordBooks.BLL.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        public Household Household { get; set; }

        public DateTimeOffset Created { get; set; }

        [Required]
        [StringLength( 40, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2 )]
        public string Subject { get; set; }

        [Required]
        [StringLength( 300, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2 )]
        public string Body { get; set; }

        public bool IsRead { get; set; }
    }
}
