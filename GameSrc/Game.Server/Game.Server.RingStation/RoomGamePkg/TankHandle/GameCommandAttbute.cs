using System;

namespace Game.Server.RingStation.RoomGamePkg.TankHandle
{
	public class GameCommandAttbute : Attribute
	{
		public byte Code
		{
			get;
			private set;
		}

		public GameCommandAttbute(byte code)
		{
			Code = code;
		}
	}
}
