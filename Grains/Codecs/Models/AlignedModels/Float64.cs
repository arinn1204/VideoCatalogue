using System.Runtime.InteropServices;

namespace Grains.Codecs.Models.AlignedModels
{
	[StructLayout(LayoutKind.Explicit)]
	public struct Float64
	{
		[FieldOffset(0)]
		public byte B1;

		[FieldOffset(1)]
		public byte B2;

		[FieldOffset(2)]
		public byte B3;

		[FieldOffset(3)]
		public byte B4;

		[FieldOffset(4)]
		public byte B5;

		[FieldOffset(5)]
		public byte B6;

		[FieldOffset(6)]
		public byte B7;

		[FieldOffset(7)]
		public byte B8;

		[FieldOffset(0)]
		public double Data;

		[FieldOffset(0)]
		public ulong UnsignedData;
	}
}