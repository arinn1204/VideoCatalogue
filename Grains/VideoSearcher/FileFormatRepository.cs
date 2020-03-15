﻿using Microsoft.Data.SqlClient;
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
            await foreach (var format in ExecuteCommand("file_name_pattern", "title_group", "year_group", "season_group", "episode_group", "container_group"))
            {
                var results = format.ToArray();
                var pattern = new Regex(results[0] as string);
                var titleGroup = (int)results[1];
                var yearGroup = results[2] as int?;
                var seasonGroup = results[3] as int?;
                var episodeGroup = results[4] as int?;
                var containerGroup = (int)results[5];

                yield return new FileFormat
                {
                    Pattern = pattern,
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
            var commandText = $"SELECT {string.Join(',', columns)} FROM video.acceptable_file_formats;";

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
