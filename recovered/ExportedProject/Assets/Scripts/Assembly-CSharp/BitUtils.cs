public static class BitUtils
{
	public static int LowestZeroBitIndex(int mask)
	{
		int num = 0;
		while ((mask & 1) != 0)
		{
			num++;
			mask >>= 1;
		}
		return num;
	}

	public static int BitCount(int mask)
	{
		int num = 0;
		while (mask != 0)
		{
			num++;
			mask &= mask - 1;
		}
		return num;
	}
}
