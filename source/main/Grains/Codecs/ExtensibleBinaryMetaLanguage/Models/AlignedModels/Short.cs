using System.Runtime.InteropServices;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.AlignedModels
{
	[StructLayout(LayoutKind.Explicit)]
	public struct Short
	{
		[FieldOffset(0)]
		public byte B1;

		[FieldOffset(1)]
		public byte B2;

		[FieldOffset(0)]
		public ushort UnsignedData;
	}
}