using System;
using System.Runtime.CompilerServices;

internal class Consortia : Attribute
{
	private byte byte_0;

	public Consortia(byte byte_1)
	{
		method_1(byte_1);
	}

	public byte method_0()
	{
		return byte_0;
	}

	[CompilerGenerated]
	private void method_1(byte byte_1)
	{
		byte_0 = byte_1;
	}
}
