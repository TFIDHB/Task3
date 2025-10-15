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

        public IEnumerable<TaskClass> GetTaskList()
        {
            using var connection = GetConnection();
            var sql = @"SELECT * FROM Tasks";
            return connection.Query<TaskClass>(sql).ToList();
        }

        public TaskClass GetTask(int id) { 
            using var connection = GetConnection();
            var sql = @"SELECT * FROM Tasks WHERE Id = @Id";
            return connection.QueryFirstOrDefault<TaskClass>(sql, new { Id = id });
        }

        public void Create(TaskClass task) { 
            using var connection = GetConnection();
            var sql = @"INSERT INTO Tasks (Title, Description, IsCompleted, CreatedAt)
                VALUES(@Title, @Description, @IsCompleted, @CreatedAt)";
            connection.Execute(sql, task);
        }

        public void Update(TaskClass task) { 
            using var connection = GetConnection();
            var sql = @"UPDATE Tasks
                SET Title = @Title, Description = @Description, IsCompleted = @IsCompleted
                WHERE Id = @Id";
            connection.Execute(sql, task);
        }

        public void Delete(int id) { 
            using var connection = GetConnection();
            var sql = @"DELETE FROM Tasks WHERE Id = @Id";
            connection.Execute(sql, new { Id = id });
        }
    }
}
