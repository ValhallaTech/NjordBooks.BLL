using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NjordBooks.BLL.Data.Enums;

namespace NjordBooks.BLL.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        // Null If It's A Deposit
        public int? CategoryItemId { get; set; }
        public CategoryItem CategoryItem { get; set; }

        public int BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }

        [StringLength(40, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string NjordBooksUserId { get; set; }
        public NjordBooksUser NjordBooksUser { get; set; }

        public DateTimeOffset Created { get; set; }

        public TransactionType Type { get; set; }

        [Required]
        [StringLength(40, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Memo { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        public bool IsDeleted { get; set; }
    }
}
