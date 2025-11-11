using System;
using System.Runtime.CompilerServices;

namespace Game.Server.HotSpringRooms.TankHandle
{
	public class HotSpringCommandAttbute : Attribute
	{
		[CompilerGenerated]
		private byte byte_0;

		public byte Code
		{
			[CompilerGenerated]
			get
			{
				return byte_0;
			}
			[CompilerGenerated]
			private set
			{
				byte_0 = value;
			}
		}

		public HotSpringCommandAttbute(byte code)
		{
			Code = code;
		}
	}
}
