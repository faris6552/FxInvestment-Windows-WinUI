using System;
using System.Collections.Generic;

namespace FxInvestmentManager.Models
{
    public class ReportParameters
    {
        public string ReportType { get; set; } = string.Empty; // "Weekly", "Monthly", "Quarterly", "Yearly", "Custom"
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AccountId { get; set; } = string.Empty; // "All" or specific account
        public bool IncludeCharts { get; set; } = true;
        public string ExportFormat { get; set; } = "PDF"; // "PDF", "Excel", "CSV"
    }

    public class ReportData
    {
        public string Title { get; set; } = string.Empty;
        public DateTime GeneratedDate { get; set; }
        public string Period { get; set; } = string.Empty;
        public decimal TotalProfitLoss { get; set; }
        public int TotalTrades { get; set; }
        public decimal WinRate { get; set; }
        public decimal AverageWin { get; set; }
        public decimal AverageLoss { get; set; }
        public decimal MaxDrawdown { get; set; }
        public Dictionary<string, decimal> AccountPerformance { get; set; } = new();
        public Dictionary<string, int> InstrumentBreakdown { get; set; } = new();
    }
}