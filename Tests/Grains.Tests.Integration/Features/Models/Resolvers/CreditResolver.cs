using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Grains.Tests.Integration.Features.Assertions
{
	public class CreditResolver : DefaultContractResolver
	{
		private readonly IDictionary<string, string> _propertyOverrides =
			new Dictionary<string, string>
			{
				["ProfilePath"] = "profile_path"
			};

		protected override IList<JsonProperty> CreateProperties(
			Type type,
			MemberSerialization memberSerialization)
		{
			var properties = base.CreateProperties(type, memberSerialization);

			// only serializer properties that are not named after the specified property.

			properties = ValidateProperty(properties)
			   .ToList();

			return properties;
		}


		private IEnumerable<JsonProperty> ValidateProperty(IList<JsonProperty> properties)
		{
			foreach (var property in properties)
			{
				property.PropertyName = _propertyOverrides.ContainsKey(property.PropertyName)
					? _propertyOverrides[property.PropertyName]
					: property.PropertyName;

				yield return property;
			}
		}
	}
}