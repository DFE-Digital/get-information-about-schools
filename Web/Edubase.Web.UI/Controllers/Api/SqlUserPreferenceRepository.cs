using Edubase.Web.UI.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edubase.Web.UI.Controllers.Api
{
    public class SqlUserPreferenceRepository : SqlRepositoryBase, ISqlUserPreferenceRepository
    {
        public async Task<SqlUserPreference> GetAsync(string userId)
        {
            using (var connection = new SqlConnection(BuildConnectionString()))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(
                    "SELECT UserId, SavedSearchToken FROM UserPreferences WHERE UserId = @userId", connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                            return new SqlUserPreference
                            {
                                UserId = reader.GetString(0),
                                SavedSearchToken = reader.IsDBNull(1) ? null : reader.GetString(1)
                            };
                    }
                }
            }
            return null;
        }

        public SqlUserPreference Get(string userId) => GetAsync(userId).GetAwaiter().GetResult();

        public async Task UpsertAsync(SqlUserPreference item)
        {
            var existing = await GetAsync(item.UserId);
            using (var connection = new SqlConnection(BuildConnectionString()))
            {
                await connection.OpenAsync();
                if (existing == null)
                {
                    using (var command = new SqlCommand(
                        "INSERT INTO UserPreferences (UserId, SavedSearchToken) VALUES (@userId, @token)", connection))
                    {
                        command.Parameters.AddWithValue("@userId", item.UserId);
                        command.Parameters.AddWithValue("@token", (object)item.SavedSearchToken ?? DBNull.Value);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                else
                {
                    using (var command = new SqlCommand(
                        "UPDATE UserPreferences SET SavedSearchToken = @token WHERE UserId = @userId", connection))
                    {
                        command.Parameters.AddWithValue("@token", (object)item.SavedSearchToken ?? DBNull.Value);
                        command.Parameters.AddWithValue("@userId", item.UserId);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        public async Task<IEnumerable<SqlUserPreference>> GetAllAsync()
        {
            var results = new List<SqlUserPreference>();
            using (var connection = new SqlConnection(BuildConnectionString()))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(
                    "SELECT UserId, SavedSearchToken FROM UserPreferences", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(new SqlUserPreference
                            {
                                UserId = reader.GetString(0),
                                SavedSearchToken = reader.IsDBNull(1) ? null : reader.GetString(1)
                            });
                        }
                    }
                }
            }
            return results;
        }
    }
}
