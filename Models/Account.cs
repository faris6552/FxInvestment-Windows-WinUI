using System;

namespace FxInvestmentManager.Models
{
    public class Account
    {
        public string AccountId { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public decimal InitialDeposit { get; set; }
        public decimal CurrentBalance { get; set; }
        public string Currency { get; set; } = "USD";
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class Transaction
    {
        public string AccountId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "DEPOSIT" or "WITHDRAWAL"
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}