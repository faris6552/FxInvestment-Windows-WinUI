using FxInvestmentManager.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FxInvestmentManager.Services
{
    public class CsvParserService
    {
        public async Task<List<TradeRecord>> ParseTradeCsvAsync(string filePath)
        {
            var trades = new List<TradeRecord>();

            try
            {
                var lines = await File.ReadAllLinesAsync(filePath);

                // Skip header line and process data
                foreach (var line in lines.Skip(1))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var columns = ParseCsvLine(line);
                    if (columns.Length >= 20) // Based on your CSV structure
                    {
                        var trade = new TradeRecord
                        {
                            TradeId = columns[0]?.Trim('"') ?? string.Empty,
                            Instrument = columns[3]?.Trim('"') ?? string.Empty,
                            ProfitLoss = decimal.TryParse(columns[16]?.Trim('"'), NumberStyles.Any, CultureInfo.InvariantCulture, out var pl) ? pl : 0,
                            Timestamp = DateTime.TryParse(columns[19]?.Trim('"'), out var ts) ? ts : DateTime.MinValue,
                            Status = columns[12]?.Trim('"') ?? string.Empty
                        };

                        // Only include closed trades
                        if (trade.Status == "CLOSED")
                        {
                            trades.Add(trade);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error parsing CSV file: {ex.Message}", ex);
            }

            return trades;
        }

        private string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            var current = "";
            var inQuotes = false;

            foreach (var c in line)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current);
                    current = "";
                }
                else
                {
                    current += c;
                }
            }

            result.Add(current);
            return result.ToArray();
        }

        public (decimal totalProfit, int totalTrades, decimal maxWin, decimal minWin) CalculateTradeStats(List<TradeRecord> trades)
        {
            if (trades == null || trades.Count == 0)
                return (0, 0, 0, 0);

            var profitableTrades = trades.Where(t => t.ProfitLoss > 0).ToList();
            var totalProfit = trades.Sum(t => t.ProfitLoss);
            var totalTrades = trades.Count;
            var maxWin = profitableTrades.Any() ? profitableTrades.Max(t => t.ProfitLoss) : 0;
            var minWin = profitableTrades.Any() ? profitableTrades.Min(t => t.ProfitLoss) : 0;

            return (totalProfit, totalTrades, maxWin, minWin);
        }
    }
}