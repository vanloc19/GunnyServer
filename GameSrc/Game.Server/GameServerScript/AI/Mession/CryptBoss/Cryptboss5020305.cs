using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class Cryptboss5020305 : AMissionControl
	{
		private SimpleBoss simpleBoss_0;

		private int fOjsdIdfmJf;

		private int int_0;

		public Cryptboss5020305()
		{
			
			this.fOjsdIdfmJf = 50215;
			
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
			this.int_0++;
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
			int[] numArray = new int[] { this.fOjsdIdfmJf };
			int[] numArray1 = new int[] { this.fOjsdIdfmJf };
			base.Game.LoadResources(numArray);
			base.Game.LoadNpcGameOverResources(numArray1);
			base.Game.AddLoadingFile(1, "bombs/13.swf", "tank.resource.bombs.Bomb13");
			base.Game.SetMap(1513);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			this.simpleBoss_0 = base.Game.CreateBoss(this.fOjsdIdfmJf, 796, 186, -1, 1, "");
			this.simpleBoss_0.SetRelateDemagemRect(this.simpleBoss_0.NpcInfo.X, this.simpleBoss_0.NpcInfo.Y, this.simpleBoss_0.NpcInfo.Width, this.simpleBoss_0.NpcInfo.Height);
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return this.int_0;
		}
	}
}