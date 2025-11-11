using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;

namespace GameServerScript.AI.Messions
{
	public class ChristmasSecond : AMissionControl
	{
		private SimpleBoss simpleBoss_0;

		private int int_0;

		private int int_1;

		public ChristmasSecond()
		{

			this.int_0 = 10013;

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
				return;
			}
			base.Game.TakeSnow();
			base.Game.IsWin = true;
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
			base.Game.SetMap(1345);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsChristmasBoss = true;
			this.simpleBoss_0 = base.Game.CreateBoss(this.int_0, 736, 793, -1, 4, "", livingConfig);
			this.simpleBoss_0.SetRelateDemagemRect(this.simpleBoss_0.NpcInfo.X, this.simpleBoss_0.NpcInfo.Y, this.simpleBoss_0.NpcInfo.Width, this.simpleBoss_0.NpcInfo.Height);
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return this.int_1;
		}
	}
}