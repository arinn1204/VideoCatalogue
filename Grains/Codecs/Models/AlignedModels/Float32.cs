using System.Runtime.InteropServices;

namespace Grains.Codecs.Models.AlignedModels
{
	[StructLayout(LayoutKind.Explicit)]
	public struct Float32
	{
		[FieldOffset(0)]
		public byte B1;

		[FieldOffset(1)]
		public byte B2;

		[FieldOffset(2)]
		public byte B3;

		[FieldOffset(3)]
		public byte B4;

		[FieldOffset(0)]
		public float Data;

		[FieldOffset(0)]
		public uint UnsignedData;

		[FieldOffset(0)]
		public int SignedData;
	}
}