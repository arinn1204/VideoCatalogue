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

        public async IAsyncEnumerable<FileFormat> GetAcceptableFileFormats()
        {
            await foreach (var format in ExecuteCommand("file_name_pattern"))
            {
                var pattern = new Regex(format.First() as string);

                yield return new FileFormat
                {
                    Pattern = pattern
                };
            };
        }
        public async IAsyncEnumerable<string> GetAllowedFileTypes()
        {
            await foreach (var format in ExecuteCommand("file_type"))
            {
                yield return format.First() as string;
            };
        }

        public IAsyncEnumerable<string> GetFilteredKeywords()
        {
            throw new NotImplementedException();
        }

        private async IAsyncEnumerable<IEnumerable<object>> ExecuteCommand(params string[] columns)
        {
            var connectionString = _configuration.GetConnectionString("VideoSearcher");
            var commandText = $"SELECT {columns.Aggregate((acc, cur) => acc + "," + cur).Trim(',')} FROM video.acceptable_file_formats;";

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
                var items = Enumerable.Empty<object>();
                for(var i = 0; i < columns.Length; i++)
                {
                    items = items.Append(reader.GetValue(i));
                }
                yield return items;
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
