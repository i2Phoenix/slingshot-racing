public static class CoreUtils
{
	public static void Swap<T>(ref T _lhs, ref T _rhs)
	{
		T val = _lhs;
		_lhs = _rhs;
		_rhs = val;
	}

	public static byte[] MakeFourCC(char _c0, char _c1, char _c2, char _c3)
	{
		return new byte[4]
		{
			(byte)_c0,
			(byte)_c1,
			(byte)_c2,
			(byte)_c3
		};
	}

	public static byte[] MakeFourCC(string _code)
	{
		if (_code.Length != 4)
		{
			return new byte[4];
		}
		return MakeFourCC(_code[0], _code[1], _code[2], _code[3]);
	}
}
