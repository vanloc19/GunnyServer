using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class Cryptboss5020103 : AMissionControl
	{
		private SimpleBoss simpleBoss_0;

		private int int_0;

		private int int_1;

		public Cryptboss5020103()
		{
			
			this.int_0 = 50203;
			
		}

		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			if (score > 1750)
			{
				return 3;
			}
			if (score > 1675)
			{
				return 2;
			}
			if (score > 1600)
			{
				return 1;
			}
			return 0;
		}

		public override bool CanGameOver()
		{
			if (this.simpleBoss_0 == null || this.simpleBoss_0.IsLiving)
			{
				return false;
			}
			this.int_1++;
			return true;
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (this.simpleBoss_0 == null || this.simpleBoss_0.IsLiving)
			{
				base.Game.IsWin = false;
			}
			else
			{
				base.Game.IsWin = true;
			}
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				allFightPlayer.PlayerDetail.UpdatePveResult("cryotboss", 1, base.Game.IsWin);
			}
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
		}

		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			int[] int0 = new int[] { this.int_0 };
			int[] numArray = new int[] { this.int_0 };
			base.Game.LoadResources(int0);
			base.Game.LoadNpcGameOverResources(numArray);
			base.Game.AddLoadingFile(2, "image/game/effect/11/064.swf", "asset.game.eleven.064");
			base.Game.SetMap(1511);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			this.simpleBoss_0 = base.Game.CreateBoss(this.int_0, 796, 526, -1, 1, "");
			this.simpleBoss_0.SetRelateDemagemRect(this.simpleBoss_0.NpcInfo.X, this.simpleBoss_0.NpcInfo.Y, this.simpleBoss_0.NpcInfo.Width, this.simpleBoss_0.NpcInfo.Height);
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return this.int_1;
		}
	}
}