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
        //All additional regexes after this will be treated as individual filters.
        //All regexes must be successful in order for the first one, the match, to match a title.
        private const string FilterKey = "&FILTER&";
        private readonly IConfiguration _configuration;

        public FileFormatRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async IAsyncEnumerable<FileFormat> GetAcceptableFileFormats()
        {
            await foreach (var format in ExecuteCommand(
                "video_file.file_patterns",
                "file_name_pattern",
                "title_group",
                "year_group",
                "season_group",
                "episode_group",
                "container_group"))
            {
                var results = format.ToArray();
                var patterns = results[0] as string;
                var titleGroup = (int)results[1];
                var yearGroup = results[2] as int?;
                var seasonGroup = results[3] as int?;
                var episodeGroup = results[4] as int?;
                var containerGroup = (int)results[5];

                yield return new FileFormat
                {
                    Patterns = patterns.Split(FilterKey).Select(s => new Regex(s)),
                    ContainerGroup = containerGroup,
                    EpisodeGroup = episodeGroup,
                    SeasonGroup = seasonGroup,
                    TitleGroup = titleGroup,
                    YearGroup   = yearGroup
                };
            };
        }
        public async IAsyncEnumerable<string> GetAllowedFileTypes()
        {
            await foreach (var format in ExecuteCommand(
                "video_file.file_types",
                "file_type"))
            {
                yield return format.First() as string;
            };
        }

        public async IAsyncEnumerable<string> GetFilteredKeywords()
        {
            await foreach (var format in ExecuteCommand(
                "video_file.filtered_keywords",
                "file_type"))
            {
                yield return format.First() as string;
            };
        }

        private async IAsyncEnumerable<IEnumerable<object>> ExecuteCommand(string table, params string[] columns)
        {
            var connectionString = _configuration.GetConnectionString("VideoSearcher");
            var commandText = $"SELECT {string.Join(',', columns)} FROM {table};";

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
