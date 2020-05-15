using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;
using Grains.FileFormat.Models;
using Grains.FileFormat.Models.Extensions;

namespace Grains.VideoFilter
{
	public class FormatEqualityComparer : IEqualityComparer<string>
	{
		private readonly IMapper _mapper;

		public FormatEqualityComparer(IMapper mapper)
		{
			_mapper = mapper;
		}

#region IEqualityComparer<string> Members

		public bool Equals(string x, string y)
		{
			var (capturePattern, stringToMatch) =
				(Deserialize(x), Deserialize(y)) switch
				{
					(Pattern pattern, null) => (_mapper.Map<CapturePattern>(pattern), y),
					(null, Pattern pattern) => (_mapper.Map<CapturePattern>(pattern), x),
					_                       => (null as CapturePattern, string.Empty)
				};

			return capturePattern?.IsMatch(stringToMatch) ?? x.Equals(y);
		}

		public int GetHashCode(string obj) => 1;

#endregion

		private Pattern? Deserialize(string attempt)
		{
			var pattern = default(Pattern);
			try
			{
				pattern = JsonSerializer.Deserialize<Pattern>(attempt);
			}
			catch (JsonException)
			{
			}

			return pattern;
		}
	}
}