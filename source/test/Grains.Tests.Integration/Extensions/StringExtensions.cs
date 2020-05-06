using System.IO;
using System.Linq;

namespace Grains.Tests.Integration.Extensions
{
	public static class StringExtensions
	{
		public static string ToFilePath(this string baseFileName, string folderName = "VideoApi")
		{
			var location = typeof(StringExtensions).Assembly.Location;
			var sourceDirectory = Directory.GetParent(location).FullName;

			return Path.Combine(
				sourceDirectory,
				"TestData",
				folderName,
				baseFileName);
		}

		public static string ToFileBaseName(this string title)
		{
			return string.Join(
				string.Empty,
				title.Split(' ')
				     .Select(
					      (word, index) =>
					      {
						      if (index == 0)
						      {
							      return word.ToLower();
						      }

						      var firstCharacter = char.ToUpper(word.First());
						      var restOfWord = string.Join(string.Empty, word.Skip(1))
						                             .ToLower();
						      return $"{firstCharacter}{restOfWord}";
					      }));
		}
	}
}