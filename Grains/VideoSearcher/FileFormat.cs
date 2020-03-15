using System.Text.RegularExpressions;

namespace Grains.VideoSearcher
{
    public class FileFormat
    {
        public Regex Pattern { get; set; }
        public int TitleGroup { get; set; }
        public int? YearGroup { get; set; }
        public int? SeasonGroup { get; set; }
        public int? EpisodeGroup { get; set; }
        public int ContainerGroup { get; set; }
    }
}