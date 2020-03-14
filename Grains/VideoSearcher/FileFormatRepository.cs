using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Grains.VideoSearcher
{
    public class FileFormatRepository : IFileFormatRepository
    {
        private readonly IConfiguration _configuration;

        public FileFormatRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async IAsyncEnumerable<Regex> GetAcceptableFileFormats()
        {
            await foreach (var format in ExecuteCommand("file_name_pattern"))
            {
                yield return new Regex(format);
            };
        }
        public async IAsyncEnumerable<string> GetAllowedFileTypes()
        {
            await foreach (var format in ExecuteCommand("file_type"))
            {
                yield return format;
            };
        }

        public IAsyncEnumerable<string> GetFilteredKeywords()
        {
            throw new NotImplementedException();
        }

        private async IAsyncEnumerable<string> ExecuteCommand(string column)
        {
            var connectionString = _configuration.GetConnectionString("VideoSearcher");
            var commandText = $"SELECT {column} FROM video.acceptable_file_formats;";

            SqlDataReader reader;
            SqlConnection sqlConnection;
            SqlCommand command;
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                command = new SqlCommand(commandText, sqlConnection);

                sqlConnection.Open();
                reader = await command.ExecuteReaderAsync();
            }
            catch (SqlException e)
            {
                throw e;
            }

            while (reader != null && reader.Read())
            {
                yield return reader.GetString(0);
            }

            var disposeTasks = AsyncEnumerable.Empty<ValueTask>();

            if (sqlConnection != null)
            {
                sqlConnection.Close();
                disposeTasks = disposeTasks.Append(sqlConnection.DisposeAsync());
            }

            if (command != null)
            {
                disposeTasks = disposeTasks.Append(command.DisposeAsync());
            }

            await foreach (var task in disposeTasks)
            {
                await task;
            }
        }
    }
}
