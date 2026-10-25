using System.Collections.Generic;

public interface ISerialiser
{
	bool IsReading { get; }

	bool IsWriting { get; }

	long Position { get; }

	bool IsValid();

	bool FourCC(byte[] _fourCC);

	void Serialise(ref bool io_value);

	void Serialise(ref byte io_value);

	void Serialise(ref char io_value);

	void Serialise(ref ushort io_value);

	void Serialise(ref short io_value);

	void Serialise(ref uint io_value);

	void Serialise(ref int io_value);

	void Serialise(ref ulong io_value);

	void Serialise(ref long io_value);

	void Serialise(ref float io_value);

	void Serialise(ref double io_value);

	void Serialise(ref string io_value);

	bool Serialise(Dictionary<string, string> _properties);

	bool BeginProperties(out IPropertySerialiser o_propertySerialiser);

	bool EndProperties();

	bool Sentinal();

	bool EndOfSerialise();
}
