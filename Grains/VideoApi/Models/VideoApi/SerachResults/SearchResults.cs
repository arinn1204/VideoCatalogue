﻿using System;
using GrainsInterfaces.Models.VideoApi.Enums;
using Newtonsoft.Json;

namespace Grains.VideoApi.Models.VideoApi.SerachResults
{
	[JsonObject]
	public class SearchResult
	{
		[JsonProperty]
		public virtual int Id { get; set; }

		[JsonProperty]
		public virtual string Title { get; set; }

		[JsonProperty("release_date")]
		public virtual DateTime ReleaseDate { get; set; }

		public MovieType Type { get; set; }
	}
}