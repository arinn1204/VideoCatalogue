﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace Grains.VideoInformation.Models.Credits
{
	public class MovieCredit
	{
		[JsonProperty]
		public int Id { get; set; }

		[JsonProperty]
		public IEnumerable<CastCredit> Cast { get; set; }

		[JsonProperty]
		public IEnumerable<CrewCredit> Crew { get; set; }
	}
}