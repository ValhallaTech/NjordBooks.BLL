using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NjordBooks.BLL.Models
{
    public class Category
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        public Household Household { get; set; }

        [Required]
        [StringLength( 40, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                       MinimumLength = 2 )]
        public string Name { get; set; }

        [Required]
        [StringLength( 40, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                       MinimumLength = 2 )]
        public string Description { get; set; }

        public ICollection<CategoryItem> CategoryItems { get; set; } = new HashSet<CategoryItem>( );
    }
}
