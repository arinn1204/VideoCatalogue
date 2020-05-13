using System.Linq;

namespace Grains.FileFormat.Models.Extensions
{
	public static class PatternExtensions
	{
		public static bool IsMatch(this CapturePattern capturePattern, string itemToMatch)
		{
			var captureMatches = capturePattern.Capture.IsMatch(itemToMatch);
			var noNegativeFilters =
				capturePattern.NegativeFilters.All(a => !a.IsMatch(itemToMatch));
			var allPositiveFilters =
				capturePattern.PositiveFilters.All(a => a.IsMatch(itemToMatch));

			return captureMatches && noNegativeFilters && allPositiveFilters;
		}
	}
}