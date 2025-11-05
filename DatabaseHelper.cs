using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace FxInvestmentManager
{
    public class DatabaseHelper
    {
        private string _connectionString = "Server=localhost;Database=FxInvestment;Uid=fxappuser;Pwd=fxpassword123;";

        // Test database connection
        public bool TestConnection()
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database connection failed: {ex.Message}");
                return false;
            }
        }

        // Execute query and return DataTable
        public async Task<DataTable> ExecuteQueryAsync(string query)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var adapter = new MySqlDataAdapter(query, connection))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Query execution failed: {ex.Message}");
                throw new Exception($"Database error: {ex.Message}", ex);
            }
        }

        // Execute non-query (INSERT, UPDATE, DELETE)
        public async Task<int> ExecuteNonQueryAsync(string query)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        return await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Non-query execution failed: {ex.Message}");
                throw new Exception($"Database error: {ex.Message}", ex);
            }
        }

        // Execute scalar query
        public async Task<object?> ExecuteScalarAsync(string query)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        return await command.ExecuteScalarAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Scalar execution failed: {ex.Message}");
                return null;
            }
        }

        // Account-related methods
        public async Task<DataTable> GetAccountsAsync()
        {
            string query = "SELECT account_id, account_name, initial_deposit, current_balance, currency, description, created_date, is_active FROM accounts WHERE is_active = TRUE ORDER BY created_date DESC";
            return await ExecuteQueryAsync(query);
        }

        public async Task<bool> CreateAccountAsync(string accountId, string accountName, decimal initialDeposit, string description = "")
        {
            string query = $@"
                INSERT INTO accounts (account_id, account_name, initial_deposit, current_balance, currency, description, created_date, is_active) 
                VALUES ('{accountId}', '{accountName}', {initialDeposit}, {initialDeposit}, 'USD', '{description}', NOW(), TRUE)";

            var result = await ExecuteNonQueryAsync(query);
            return result > 0;
        }

        public async Task<bool> UpdateAccountBalanceAsync(string accountId, decimal newBalance)
        {
            string query = $"UPDATE accounts SET current_balance = {newBalance} WHERE account_id = '{accountId}'";
            var result = await ExecuteNonQueryAsync(query);
            return result > 0;
        }

        // Transaction-related methods
        public async Task<bool> RecordTransactionAsync(string accountId, string type, decimal amount, string description = "")
        {
            string query = $@"
                INSERT INTO transactions (account_id, type, amount, description, transaction_date) 
                VALUES ('{accountId}', '{type}', {amount}, '{description}', NOW())";

            var result = await ExecuteNonQueryAsync(query);
            return result > 0;
        }

        public async Task<DataTable> GetTransactionsAsync(string accountId = "ALL", int limit = 100)
        {
            string query = accountId == "ALL"
                ? $"SELECT account_id, type, amount, description, transaction_date FROM transactions ORDER BY transaction_date DESC LIMIT {limit}"
                : $"SELECT account_id, type, amount, description, transaction_date FROM transactions WHERE account_id = '{accountId}' ORDER BY transaction_date DESC LIMIT {limit}";

            return await ExecuteQueryAsync(query);
        }

        // Performance-related methods
        public async Task<DataTable> GetPerformanceRecordsAsync(string accountId = "ALL")
        {
            string query = accountId == "ALL"
                ? "SELECT fxid, account_base, week, month, year, results, datetime, comments, total_trades, total_profit, max_win, min_win FROM performance ORDER BY datetime DESC"
                : $"SELECT fxid, account_base, week, month, year, results, datetime, comments, total_trades, total_profit, max_win, min_win FROM performance WHERE account_base = '{accountId}' ORDER BY datetime DESC";

            return await ExecuteQueryAsync(query);
        }

        public async Task<bool> CreatePerformanceRecordAsync(string fxid, string accountBase, int week, int month, int year, decimal results, string comments, int totalTrades, decimal totalProfit, decimal maxWin, decimal minWin, string filePath = "")
        {
            string query = $@"
                INSERT INTO performance (fxid, account_base, week, month, year, results, datetime, comments, file_path, total_trades, total_profit, max_win, min_win) 
                VALUES ('{fxid}', '{accountBase}', {week}, {month}, {year}, {results}, NOW(), '{comments}', '{filePath}', {totalTrades}, {totalProfit}, {maxWin}, {minWin})";

            var result = await ExecuteNonQueryAsync(query);
            return result > 0;
        }

        public async Task<bool> UpdatePerformanceRecordAsync(string fxid, decimal results, string comments, int totalTrades, decimal totalProfit, decimal maxWin, decimal minWin)
        {
            string query = $@"
                UPDATE performance 
                SET results = {results}, comments = '{comments}', total_trades = {totalTrades}, total_profit = {totalProfit}, max_win = {maxWin}, min_win = {minWin}, updated_at = NOW() 
                WHERE fxid = '{fxid}'";

            var result = await ExecuteNonQueryAsync(query);
            return result > 0;
        }

        public async Task<bool> DeletePerformanceRecordAsync(string fxid)
        {
            string query = $"DELETE FROM performance WHERE fxid = '{fxid}'";
            var result = await ExecuteNonQueryAsync(query);
            return result > 0;
        }

        // Dashboard statistics
        public async Task<decimal> GetTotalBalanceAsync()
        {
            string query = "SELECT SUM(current_balance) FROM accounts WHERE is_active = TRUE";
            var result = await ExecuteScalarAsync(query);
            return result != null ? Convert.ToDecimal(result) : 0;
        }

        public async Task<int> GetActiveAccountsCountAsync()
        {
            string query = "SELECT COUNT(*) FROM accounts WHERE is_active = TRUE";
            var result = await ExecuteScalarAsync(query);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public async Task<decimal> GetWeeklyProfitLossAsync()
        {
            string query = @"
                SELECT SUM(results) 
                FROM performance 
                WHERE YEAR(datetime) = YEAR(NOW()) 
                AND WEEK(datetime) = WEEK(NOW())";

            var result = await ExecuteScalarAsync(query);
            return result != null ? Convert.ToDecimal(result) : 0;
        }

        public async Task<int> GetWeeklyTradeCountAsync()
        {
            string query = @"
                SELECT SUM(total_trades) 
                FROM performance 
                WHERE YEAR(datetime) = YEAR(NOW()) 
                AND WEEK(datetime) = WEEK(NOW())";

            var result = await ExecuteScalarAsync(query);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        // Utility methods
        public async Task<bool> InitializeDatabaseAsync()
        {
            try
            {
                // Check if tables exist and create them if not
                string checkTablesQuery = @"
                    SELECT COUNT(*) 
                    FROM information_schema.tables 
                    WHERE table_schema = 'FxInvestment' 
                    AND table_name IN ('accounts', 'performance', 'transactions')";

                var tableCount = await ExecuteScalarAsync(checkTablesQuery);
                if (tableCount != null && Convert.ToInt32(tableCount) == 3)
                {
                    return true; // Tables already exist
                }

                // If tables don't exist, you could create them here
                // For now, we'll assume they exist from your SQL script
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database initialization failed: {ex.Message}");
                return false;
            }
        }

        public string GetConnectionStatus()
        {
            return TestConnection() ? "🟢 Database: Connected" : "🔴 Database: Disconnected";
        }

        public string GetTestData()
        {
            return TestConnection()
                ? "Database connection successful - Ready for operations"
                : "Database connection failed - Running in offline mode";
        }
    }
}