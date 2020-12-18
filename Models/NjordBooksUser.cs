using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace NjordBooks.BLL.Models
{
    public class NjordBooksUser : IdentityUser
    {
        [Required]
        [StringLength( 50 )]
        [Display( Name = "First Name" )]
        public string FirstName { get; set; }

        [Required]
        [StringLength( 50 )]
        [Display( Name = "Last Name" )]
        public string LastName { get; set; }

        [Display( Name = "Full Name" )]
        [NotMapped]
        public string FullName => $"{this.FirstName} {this.LastName}";

        public byte[] FileData { get; set; }
        public string FileName { get; set; }

        public int? HouseholdId { get; set; }
        public Household Household { get; set; }

        public ICollection<BankAccount> BankAccounts { get; set; } = new HashSet<BankAccount>( );
        public ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>( );
    }
}
