using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Task3.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;


namespace Task3.Infrastructure
{
    public class Repository : IRepository
    {

        private readonly string _connectionString;

        public Repository(IConfiguration configuration) {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private SqlConnection GetConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<TaskClass>> GetTaskListAsync()
        {
            using var connection = GetConnection();
            var sql = @"SELECT * FROM Tasks";
            var result = await connection.QueryAsync<TaskClass>(sql);
            return result.ToList();
        }

        public async Task<TaskClass> GetTaskAsync(int id) { 
            using var connection = GetConnection();
            var sql = @"SELECT * FROM Tasks WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<TaskClass>(sql, new { Id = id });
        }

        public async Task CreateAsync(TaskClass task) { 
            using var connection = GetConnection();
            var sql = @"INSERT INTO Tasks (Title, Description, IsCompleted, CreatedAt)
                VALUES(@Title, @Description, @IsCompleted, @CreatedAt)";
             await connection.ExecuteAsync(sql, task);
        }

        public async Task UpdateAsync(TaskClass task) { 
            using var connection = GetConnection();
            var sql = @"UPDATE Tasks
                SET Title = @Title, Description = @Description, IsCompleted = @IsCompleted
                WHERE Id = @Id";
            await connection.ExecuteAsync(sql, task);
        }

        public async Task DeleteAsync(int id) { 
            using var connection = GetConnection();
            var sql = @"DELETE FROM Tasks WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
