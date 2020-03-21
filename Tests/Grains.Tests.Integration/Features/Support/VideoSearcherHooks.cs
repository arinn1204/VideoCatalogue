using Grains.Helpers.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Support
{
    [Binding]
    public static class VideoSearcherHooks
    {
        public const string DataDirectory = "VideoSearcherDataLocation";

        [BeforeScenario("VideoSearcher")]
        public static void AddAcceptableFileFormats(IConfiguration configuration)
        {
            var filePattern = @"(.*(?=\(\d{3,4}\)))\s*\((\d{4})\).*\.([a-zA-Z]{3,4})$&FILTER&^(?:(?![sS]\d{1,2}[eE]\d{1,2}).)*$";

            ExecuteCommand(
                configuration,
                @$"
INSERT INTO video_file.filtered_keywords(keyword, created_time, created_by) VALUES('BLURAY', '{DateTime.Now}', '{nameof(VideoSearcherHooks)}');
INSERT INTO video_file.file_types(file_type, created_time, created_by) VALUES('mkv', '{DateTime.Now}', '{nameof(VideoSearcherHooks)}');
INSERT INTO video_file.file_patterns(file_name_pattern, title_group, year_group, container_group, created_time, created_by) VALUES
    ('{filePattern}', 1, 2, 3, '{DateTime.Now}', '{nameof(VideoSearcherHooks)}');");
        }

        [AfterScenario("VideoSearcher")]
        public static void RemoveFileFormats(IConfiguration configuration)
        {
            ExecuteCommand(
                configuration,
                @$"
DELETE FROM video_file.filtered_keywords WHERE created_by = '{nameof(VideoSearcherHooks)}';
DELETE FROM video_file.file_types WHERE created_by = '{nameof(VideoSearcherHooks)}';
DELETE FROM video_file.file_patterns WHERE created_by = '{nameof(VideoSearcherHooks)}';");
        }

        [AfterScenario("VideoSearcher", Order = int.MaxValue - 1)]
        public static void RemoveDataDirectory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Directory.Delete(DataDirectory, true);
        }

        private static void ExecuteCommand(IConfiguration configuration, string commandText)
        {
            var connectionString = configuration.CreateConnectionString("VideoSearcher");
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            using var command = new SqlCommand(commandText, connection);
            command.ExecuteNonQuery();
        }
    }
}
