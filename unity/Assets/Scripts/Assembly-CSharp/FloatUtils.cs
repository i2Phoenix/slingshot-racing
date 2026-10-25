using System.Runtime.InteropServices;

public static class FloatUtils
{
	[StructLayout((LayoutKind)2)]
	private struct Union
	{
		[FieldOffset(0)]
		public uint i;

		[FieldOffset(0)]
		public float f;
	}

	public static uint FloatToBits(float f)
	{
		Union union = default(Union);
		union.i = 0u;
		union.f = f;
		return union.i;
	}

	public static float BitsToFloat(uint i)
	{
		Union union = default(Union);
		union.f = 0f;
		union.i = i;
		return union.f;
	}
}
