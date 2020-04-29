using System.IO;
using System.Linq;

namespace Grains.Tests.Integration.Extensions
{
	public static class StringExtensions
	{
		public static string ToFilePath(this string baseFileName)
			=> Path.Combine("TestData", "VideoApi", baseFileName);

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