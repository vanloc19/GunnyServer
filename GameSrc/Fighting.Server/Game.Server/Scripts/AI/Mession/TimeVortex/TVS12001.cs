using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class TVS12001 : AMissionControl
	{
		private List<SimpleNpc> npcList = new List<SimpleNpc>();

		private int BossID = 12003;

		private int BigVolverineNPC = 12002;

		private int VolverineNPC = 12001;

		private int ChickenHelp = 12004;

		private SimpleBoss VolverineBOSS;

		private SimpleBoss HelperNPC;

		private int CountBigVolverine = 2;

		private int CountVolverine = 3;

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
			base.CanGameOver();
			if (HelperNPC.IsLiving && base.Game.TurnIndex < base.Game.MissionInfo.TotalTurn && base.Game.TotalKillCount < base.Game.MissionInfo.TotalCount)
			{
				return false;
			}
			return true;
		}

		private void CreateVolverineBoss()
		{
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.CanTakeDamage = false;
			livingConfig.CanFrost = true;
			this.VolverineBOSS = base.Game.CreateBoss(this.BossID, 55, 770, 1, 0, "", livingConfig);
			this.VolverineBOSS.SetRelateDemagemRect(this.VolverineBOSS.NpcInfo.X, this.VolverineBOSS.NpcInfo.Y, this.VolverineBOSS.NpcInfo.Width, this.VolverineBOSS.NpcInfo.Height);
			this.Game.SendGameFocus((Physics)VolverineBOSS, 1, 1);
			VolverineBOSS.Say("Я покажу вам истинную мощь!", 0, 100, 2000);

		}

		private void CreateNPC()
		{
			for (int i = 0; i < this.CountBigVolverine; i++)
			{
				this.npcList.Add(base.Game.CreateNpc(this.BigVolverineNPC, 1735, 766, 1, -1));
			}
			for (int j = 0; j < this.CountVolverine; j++)
			{
				this.npcList.Add(base.Game.CreateNpc(this.VolverineNPC, 1735, 766, 1, -1));
			}
		}

		private int CanCreateNPC()
		{
			int num = 0;
			foreach (SimpleNpc list0 in this.npcList)
			{
				if (!list0.IsLiving)
				{
					continue;
				}
				num++;
			}
			return num;
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (base.Game.TotalKillCount < base.Game.MissionInfo.TotalCount)
			{
				base.Game.IsWin = false;
			}
			else
			{
				if (this.HelperNPC.IsLiving)
				{
					base.Game.SendObjectFocus(this.VolverineBOSS, 1, 1, 1);
					this.VolverineBOSS.PlayMovie("die", 500, 500);
					this.VolverineBOSS.Say("Наших бьют!", 0, 500, 1000);
					Game.WaitTime(1500);
					base.Game.IsWin = true;
				}
			}
		}

		public override void OnGameOverMovie()
		{
			base.OnGameOverMovie();
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
			if (base.Game.TotalKillCount >= 5 && this.VolverineBOSS == null)
			{
				this.CreateVolverineBoss();
			}
			if (this.CanCreateNPC() <= 0)
			{
				base.Game.PveGameDelay = 0;
				this.CreateNPC();
			}
		}

		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			int[] int1 = new int[] { this.BigVolverineNPC, this.VolverineNPC, this.ChickenHelp, this.BossID };
			int[] numArray = new int[] { this.BigVolverineNPC, this.VolverineNPC, this.ChickenHelp, this.BossID };
			base.Game.LoadResources(int1);
			base.Game.LoadNpcGameOverResources(numArray);
			base.Game.SetMap(1207);
		}
		public override void OnPrepareStartGame()
		{
			base.OnPrepareStartGame();
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsHelper = true;
			livingConfig.IsTurn = false;
			livingConfig.CanCountKill = false;
			livingConfig.isBotom = 0;
			this.HelperNPC = base.Game.CreateBoss(this.ChickenHelp, 850, 777, -1, 0, "", livingConfig);

		}
		public override void OnStartGame()
		{
			base.OnStartGame();
			this.Game.SendGameFocus(HelperNPC, 1, 1);
			this.CreateNPC();
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return base.Game.TotalKillCount;
		}
	}
}