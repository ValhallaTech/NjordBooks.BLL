using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NjordBooks.BLL.Models
{
    public class CategoryItem
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [Required]
        [StringLength( 40, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2 )]
        public string Name { get; set; }

        [Required]
        [StringLength( 40, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2 )]
        public string Description { get; set; }

        [DataType( DataType.Currency )]
        [Column( TypeName = "decimal(6,2)" )]
        public decimal TargetAmount { get; set; }
        [DataType( DataType.Currency )]
        [Column( TypeName = "decimal(6,2)" )]
        public decimal ActualAmount { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>( );
    }
}
