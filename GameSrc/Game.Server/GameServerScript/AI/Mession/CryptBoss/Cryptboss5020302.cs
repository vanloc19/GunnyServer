using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class Cryptboss5020302 : AMissionControl
	{
		private SimpleBoss okhsdeGpghW;

		private int int_0;

		private int int_1;

		public Cryptboss5020302()
		{
			
			this.int_0 = 50212;
			
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
			if (this.okhsdeGpghW == null || this.okhsdeGpghW.IsLiving)
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
			if (this.okhsdeGpghW == null || this.okhsdeGpghW.IsLiving)
			{
				base.Game.IsWin = false;
			}
			else
			{
				base.Game.IsWin = true;
			}
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				allFightPlayer.PlayerDetail.UpdatePveResult("cryotboss", 3, base.Game.IsWin);
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
			base.Game.AddLoadingFile(1, "bombs/13.swf", "tank.resource.bombs.Bomb13");
			base.Game.SetMap(1513);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			this.okhsdeGpghW = base.Game.CreateBoss(this.int_0, 796, 186, -1, 1, "");
			this.okhsdeGpghW.SetRelateDemagemRect(this.okhsdeGpghW.NpcInfo.X, this.okhsdeGpghW.NpcInfo.Y, this.okhsdeGpghW.NpcInfo.Width, this.okhsdeGpghW.NpcInfo.Height);
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return this.int_1;
		}
	}
}