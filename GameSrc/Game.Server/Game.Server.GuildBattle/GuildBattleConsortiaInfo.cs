using System;
using System.Drawing;

namespace Game.Server.GuildBattle
{
	[Serializable]
	public class GuildBattleConsortiaInfo
	{
		public int ConsortiaID
		{
			get;
			set;
		}

		public string ConsortiaName
		{
			get;
			set;
		}

		public Point DefaultPoint
		{
			get;
			set;
		}

		public int Rank
		{
			get;
			set;
		}

		public int Score
		{
			get;
			set;
		}

		public GuildBattleConsortiaInfo()
		{
		}
	}
}