using System;

namespace FxInvestmentManager.Services
{
    public class FxIdGenerator
    {
        public string GenerateFxId(string accountId, DateTime tradeDate)
        {
            var week = GetWeekOfMonth(tradeDate);
            var month = tradeDate.Month;

            return $"{accountId}WK{week:D2}{month:D2}";
        }

        private int GetWeekOfMonth(DateTime date)
        {
            var beginningOfMonth = new DateTime(date.Year, date.Month, 1);

            while (date.Date.AddDays(1).DayOfWeek != DayOfWeek.Monday)
            {
                date = date.AddDays(1);
            }

            return (int)Math.Ceiling((date - beginningOfMonth).TotalDays / 7.0);
        }

        public (int week, int month, int year) GetWeekMonthYear(DateTime date)
        {
            var week = GetWeekOfMonth(date);
            return (week, date.Month, date.Year);
        }
    }
}