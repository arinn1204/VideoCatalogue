using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Grains.Tests.Integration.Features.Models.Resolvers
{
	public class VideoDetailResolver : DefaultContractResolver
	{
		protected override string ResolvePropertyName(string propertyName)
		{
			var propertyLookup = new Dictionary<string, string>
			                     {
				                     ["ProductionCompanies"] = "production_companies",
				                     ["ReleaseDate"] = "release_date",
				                     ["ImdbId"] = "imdb_id",
				                     ["LogoPath"] = "logo_path",
				                     ["OriginCountry"] = "origin_country",
				                     ["TmdbId"] = "id"
			                     };

			return propertyLookup.ContainsKey(propertyName)
				? propertyLookup[propertyName]
				: propertyName;
		}

		protected override JsonProperty CreateProperty(
			MemberInfo member,
			MemberSerialization memberSerialization)
		{
			var property = base.CreateProperty(member, memberSerialization);

			if (property.PropertyName == "Genres" &&
			    property.PropertyType == typeof(IEnumerable<string>))
			{
				property.Converter = new GenreDetailConverter();
			}

			return property;
		}

#region Nested type: GenreDetailConverter

		private class GenreDetailConverter : JsonConverter<IEnumerable<string>>
		{
			public override IEnumerable<string> ReadJson(
				JsonReader reader,
				Type objectType,
				[AllowNull] IEnumerable<string> existingValue,
				bool hasExistingValue,
				JsonSerializer serializer)
			{
				var value = JArray.Load(reader);

				return value.Select(
					s => s["name"]
					   .ToString());
			}

			public override void WriteJson(
				JsonWriter writer,
				IEnumerable<string> value,
				JsonSerializer serializer)
			{
				var genreArray = JArray.FromObject(value)
				                       .Select(
					                        s => new
					                             {
						                             Name = s
					                             });

				serializer.Serialize(writer, genreArray);
			}
		}

#endregion
	}
}