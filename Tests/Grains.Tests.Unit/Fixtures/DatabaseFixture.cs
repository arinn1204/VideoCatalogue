using Grains.Helpers.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
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

        public SqlCommand AddAcceptableFileFormat(IEnumerable<string> columns, IEnumerable<object> values)
        {
            var indexedColumns = columns.Select((columnName, columnIndex) => (columnName, columnIndex));
            var indexedValues = values.Select((columnValue, valueIndex) => (columnValue, valueIndex));

            var pairedValues = indexedColumns.Join<(string, int),(object, int), int, (string, object)>(
                indexedValues,
                left => left.Item2,
                right => right.Item2,
                (left, right) =>
                {
                    return ($"@{left.Item1.ToUpperInvariant()}", right.Item1 ?? DBNull.Value);
                }
            );

            var adjustedColumns = string.Join(',', columns);
            var adjustedValues = $"@{string.Join(", @", columns.Select(s => s.ToUpperInvariant()))}";

            var commandText = $"INSERT INTO video.acceptable_file_formats({adjustedColumns}, created, created_by) VALUES({adjustedValues}, '{DateTime.Now}', '{nameof(DatabaseFixture)}')";

            var command = new SqlCommand(commandText, _connection);

            foreach (var (parameterName, value) in pairedValues)
            {
                var type = value switch
                {
                    int _ => SqlDbType.Int,
                    DateTime _ => SqlDbType.DateTime,
                    _ => SqlDbType.VarChar
                };
                

                var parameter = new SqlParameter(parameterName, type)
                {
                    Value = value
                };
                command.Parameters.Add(parameter);
            }

            return command;
        }

        public void ResetTables()
        {
            using var command = new SqlCommand($"DELETE FROM video.acceptable_file_formats WHERE created_by = '{nameof(DatabaseFixture)}'", _connection);
            command.ExecuteNonQuery();
        }
    }
}
