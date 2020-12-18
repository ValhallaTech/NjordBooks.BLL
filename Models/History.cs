using System;

namespace NjordBooks.BLL.Models
{
    public class History
    {
        public int Id { get; set; }
        public int BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }
        public decimal Balance { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
