using System.Threading;

namespace Game.Server.RingStation
{
	public class RingStationConfiguration
	{
		public static int PlayerID = 3000;

		public static string[] RandomName = new string[15]
		{
			"♠ Mimi",
			"❋ℳøøn",
			"<~|RÖŸ|~>",
			"G-daggon",
			"`Daenery",
			"✎ℕαmeԼess",
			"♔TђỏNɠọɕ﹏",
			"I’AM☞Candy",
			"₉₁₁Snow",
			"✗✦ɮoɱ",
			"✎⌢Pisces",
			"♚✼ßột✼♚",
			"♚ßluɛ廴abel",
			"Çɦờξm¹tí",
			"♚ƬiêɳƘéϯ"
		};

		public static int roomID = 3000;

		public static int ServerID = 4;

		public static string ServerName = "AutoBot";

		public static int NextPlayerID()
		{
			return Interlocked.Increment(ref PlayerID);
		}

		public static int NextRoomId()
		{
			return Interlocked.Increment(ref roomID);
		}
	}
}
