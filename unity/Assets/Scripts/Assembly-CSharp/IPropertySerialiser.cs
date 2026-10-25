public interface IPropertySerialiser
{
	bool TrySerialise(string _key, ref bool io_value);

	bool TrySerialise(string _key, ref byte io_value);

	bool TrySerialise(string _key, ref char io_value);

	bool TrySerialise(string _key, ref ushort io_value);

	bool TrySerialise(string _key, ref short io_value);

	bool TrySerialise(string _key, ref uint io_value);

	bool TrySerialise(string _key, ref int io_value);

	bool TrySerialise(string _key, ref ulong io_value);

	bool TrySerialise(string _key, ref long io_value);

	bool TrySerialise(string _key, ref float io_value);

	bool TrySerialise(string _key, ref double io_value);

	bool TrySerialise(string _key, ref string io_value);
}
