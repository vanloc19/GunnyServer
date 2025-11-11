using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class Cryptboss5020504 : AMissionControl
	{
		private SimpleBoss jglsdaMsiue;

		private int int_0;

		private int int_1;

		public Cryptboss5020504()
		{
			
			this.int_0 = 50224;
			
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
			if (this.jglsdaMsiue == null || this.jglsdaMsiue.IsLiving)
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
			if (this.jglsdaMsiue == null || this.jglsdaMsiue.IsLiving)
			{
				base.Game.IsWin = false;
			}
			else
			{
				base.Game.IsWin = true;
			}
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				allFightPlayer.PlayerDetail.UpdatePveResult("cryotboss", 5, base.Game.IsWin);
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
			base.Game.AddLoadingFile(2, "image/game/effect/11/057.swf", "asset.game.eleven.057");
			base.Game.SetMap(1515);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			this.jglsdaMsiue = base.Game.CreateBoss(this.int_0, 796, 526, -1, 1, "");
			this.jglsdaMsiue.SetRelateDemagemRect(this.jglsdaMsiue.NpcInfo.X, this.jglsdaMsiue.NpcInfo.Y, this.jglsdaMsiue.NpcInfo.Width, this.jglsdaMsiue.NpcInfo.Height);
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return this.int_1;
		}
	}
}