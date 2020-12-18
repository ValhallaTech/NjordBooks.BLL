using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NjordBooks.BLL.Data.Enums;

namespace NjordBooks.BLL.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        public Household Household { get; set; }

        [StringLength( 40, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                       MinimumLength = 2 )]
        public string NjordBooksUserId { get; set; }

        public NjordBooksUser NjordBooksUser { get; set; }

        [Required]
        [StringLength( 40, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                       MinimumLength = 2 )]
        public string Name { get; set; }

        [EnumDataType( typeof( AccountType ) )]
        public AccountType Type { get; set; }

        [DataType( DataType.Currency )]
        [Column( TypeName = "decimal(10,2)" )]
        public decimal StartingBalance { get; set; }

        [DataType( DataType.Currency )]
        [Column( TypeName = "decimal(10,2)" )]
        public decimal? CurrentBalance { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>( );
        public ICollection<History> Histories { get; set; } = new HashSet<History>( );
    }
}
