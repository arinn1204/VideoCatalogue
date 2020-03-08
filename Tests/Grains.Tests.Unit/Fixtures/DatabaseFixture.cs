using Grains.Helpers.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Grains.Tests.Unit.Fixtures
{
    public class DatabaseFixture : IDisposable
    {
        private readonly SqlConnection _connection;
        public DatabaseFixture()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var connectionString = Configuration
                .CreateConnectionString("VideoSearcher");
            _connection = new SqlConnection(connectionString);
            _connection.Open();
        }

        public IConfiguration Configuration { get; }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }

        public SqlCommand AddAcceptableFileFormat(string column, string value)
        {
            var commandText = $"INSERT INTO video.acceptable_file_formats({column}, created, created_by) VALUES('{value}', '{DateTime.Now}', '{nameof(DatabaseFixture)}')";

            return new SqlCommand(commandText, _connection);
        }

        public void ResetTables()
        {
            using var command = new SqlCommand($"DELETE FROM video.acceptable_file_formats WHERE created_by = '{nameof(DatabaseFixture)}'", _connection);
            command.ExecuteNonQuery();
        }
    }
}
