﻿using System;
using System.Collections.Generic;

namespace Grains.Tests.Unit.Extensions
{
	public static class StringExtensions
	{
		public static IEnumerable<byte> ToBytes(this string id)
		{
			id = id.Replace("0x", string.Empty, StringComparison.OrdinalIgnoreCase);
			for (var i = 0; i < id.Length; i += 2)
			{
				yield return Convert.ToByte($"{id[i]}{id[i + 1]}", 16);
			}
		}
	}
}