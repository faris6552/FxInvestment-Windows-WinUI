using System;

namespace FxInvestmentManager.Models
{
    public class PerformanceRecord
    {
        public string FxId { get; set; } = string.Empty;
        public string AccountId { get; set; } = string.Empty;
        public int Week { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Results { get; set; }
        public DateTime DateTime { get; set; }
        public string? Comments { get; set; }
        public string? FilePath { get; set; }
        public int TotalTrades { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal MaxWin { get; set; }
        public decimal MinWin { get; set; }
        public string? AccountType { get; set; }
    }

    public class TradeRecord
    {
        public string TradeId { get; set; } = string.Empty;
        public string Instrument { get; set; } = string.Empty;
        public decimal ProfitLoss { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}