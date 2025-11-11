using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Game.Server.GuildBattle
{
	[Serializable]
	public class UserGuildBattleInfo
	{
		[NonSerialized]
		private GamePlayer gamePlayer_0;

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

		public bool DupeScoreConsortiaBattle
		{
			get;
			set;
		}

		public int FailBuffCount
		{
			get;
			set;
		}

		public bool IsActive
		{
			get;
			set;
		}

		public bool IsDead
		{
			get;
			set;
		}

		public int LostCount
		{
			get;
			set;
		}

		public string NickName
		{
			get;
			set;
		}

		public GamePlayer Player
		{
			get
			{
				return this.gamePlayer_0;
			}
			set
			{
				this.gamePlayer_0 = value;
			}
		}

		public Point Postion
		{
			get;
			set;
		}

		public int Score
		{
			get;
			set;
		}

		public UserGuildBattleStatus Status
		{
			get;
			set;
		}

		public DateTime TombStoneEndTime
		{
			get;
			set;
		}

		public int UserID
		{
			get;
			set;
		}

		public int VictoryCount
		{
			get;
			set;
		}

		public int WinStreak
		{
			get;
			set;
		}

		public UserGuildBattleInfo(GamePlayer player)
		{


			this.UserID = player.PlayerId;
			this.NickName = player.PlayerCharacter.NickName;
			this.ConsortiaID = player.PlayerCharacter.ConsortiaID;
			this.ConsortiaName = player.PlayerCharacter.ConsortiaName;
			this.Postion = new Point(0, 0);
			this.Status = UserGuildBattleStatus.NORMAL;
			this.TombStoneEndTime = DateTime.Now.AddHours(-1);
			player.PlayerCharacter.ReduceStartBlood = player.PlayerCharacter.hp;
			this.IsActive = true;
			this.IsDead = false;
			this.DupeScoreConsortiaBattle = false;
			this.Player = player;
		}
	}
}