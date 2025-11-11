using System;

namespace Game.Server.RingStation.RoomGamePkg
{
	public class GameProcessorAttribute : Attribute
	{
		private byte _code;

		private string _descript;

		public byte Code => _code;

		public string Description => _descript;

		public GameProcessorAttribute(byte code, string description)
		{
			_code = code;
			_descript = description;
		}
	}
}
