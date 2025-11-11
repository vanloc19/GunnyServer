using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class LauDaiPhepThuatS71055 : AMissionControl
	{
		private List<SimpleNpc> someNpc = new List<SimpleNpc>();

		public LivingConfig config = null;

		private int dieRedCount = 0;

		private int[] npcIDs = new int[]
		{
			71056,
			71055
		};

		private int[] birthX = new int[]
		{
			635,
			750,
			900,
			1115
		};

		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			int result;
			if (score > 1870)
			{
				result = 3;
			}
			else if (score > 1825)
			{
				result = 2;
			}
			else if (score > 1780)
			{
				result = 1;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			int[] npcIds = new int[]
			{
				this.npcIDs[0],
				this.npcIDs[1]
			};
			int[] npcIds2 = new int[]
			{
				this.npcIDs[1],
				this.npcIDs[0],
				this.npcIDs[0],
				this.npcIDs[0]
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.SetMap(1464);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			base.Game.IsBossWar = "16401";
			this.config = base.Game.BaseLivingConfig();
			this.config.IsFly = true;
			int num = base.Game.Random.Next(0, this.npcIDs.Length);
			int x = base.Game.Random.Next(572, 1115);
			int y = base.Game.Random.Next(510, 650);
			if (this.npcIDs[num] == 71055)
			{
				this.someNpc.Add(base.Game.CreateNpc(this.npcIDs[num], x, y, 1, -1, this.config));
			}
			else
			{
				this.someNpc.Add(base.Game.CreateNpc(this.npcIDs[num], x, y, 1, -1));
			}
			num = base.Game.Random.Next(0, this.npcIDs.Length);
			x = base.Game.Random.Next(572, 1115);
			y = base.Game.Random.Next(510, 650);
			if (this.npcIDs[num] == 71055)
			{
				this.someNpc.Add(base.Game.CreateNpc(this.npcIDs[num], x, y, 1, -1, this.config));
			}
			else
			{
				this.someNpc.Add(base.Game.CreateNpc(this.npcIDs[num], x, y, 1, -1));
			}
			num = base.Game.Random.Next(0, this.npcIDs.Length);
			x = base.Game.Random.Next(572, 1115);
			y = base.Game.Random.Next(510, 650);
			if (this.npcIDs[num] == 71055)
			{
				this.someNpc.Add(base.Game.CreateNpc(this.npcIDs[num], x, y, 1, -1, this.config));
			}
			else
			{
				this.someNpc.Add(base.Game.CreateNpc(this.npcIDs[num], x, y, 1, -1));
			}
			num = base.Game.Random.Next(0, this.npcIDs.Length);
			x = base.Game.Random.Next(572, 1115);
			y = base.Game.Random.Next(510, 650);
			if (this.npcIDs[num] == 71055)
			{
				this.someNpc.Add(base.Game.CreateNpc(this.npcIDs[num], x, y, 1, -1, this.config));
			}
			else
			{
				this.someNpc.Add(base.Game.CreateNpc(this.npcIDs[num], x, y, 1, -1));
			}
			num = base.Game.Random.Next(0, this.npcIDs.Length);
			x = base.Game.Random.Next(572, 1115);
			y = base.Game.Random.Next(510, 650);
			if (this.npcIDs[num] == 71055)
			{
				this.someNpc.Add(base.Game.CreateNpc(this.npcIDs[num], x, y, 1, -1, this.config));
			}
			else
			{
				this.someNpc.Add(base.Game.CreateNpc(this.npcIDs[num], x, y, 1, -1));
			}
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
			if (base.Game.GetLivedLivings().Count == 0)
			{
				base.Game.PveGameDelay = 0;
			}
			if (base.Game.TurnIndex > 1 && base.Game.CurrentPlayer.Delay > base.Game.PveGameDelay)
			{
				if (base.Game.GetLivedLivings().Count < 10)
				{
					for (int i = 0; i < 10 - base.Game.GetLivedLivings().Count; i++)
					{
						if (this.someNpc.Count == base.Game.MissionInfo.TotalCount)
						{
							break;
						}
						int num = base.Game.Random.Next(0, this.birthX.Length);
						int num2 = this.birthX[num];
						int x = base.Game.Random.Next(572, 1115);
						int y = base.Game.Random.Next(510, 650);
						num = base.Game.Random.Next(0, this.npcIDs.Length);
						if (num == 1 && this.GetNpcCountByID(this.npcIDs[1]) < 10)
						{
							this.someNpc.Add(base.Game.CreateNpc(this.npcIDs[1], x, y, 1, 1, this.config));
						}
						else
						{
							x = base.Game.Random.Next(1164, 1465);
							this.someNpc.Add(base.Game.CreateNpc(this.npcIDs[0], x, 911, 1, 1));
						}
					}
				}
			}
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		public override bool CanGameOver()
		{
			bool flag = true;
			base.CanGameOver();
			this.dieRedCount = 0;
			foreach (SimpleNpc current in this.someNpc)
			{
				if (current.IsLiving)
				{
					flag = false;
				}
				else
				{
					this.dieRedCount++;
				}
			}
			bool result;
			if (flag && this.dieRedCount == base.Game.MissionInfo.TotalCount)
			{
				base.Game.IsWin = true;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int UpdateUIData()
		{
			return base.Game.TotalKillCount;
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (base.Game.GetLivedLivings().Count == 0)
			{
				base.Game.IsWin = true;
				List<LoadingFileInfo> list = new List<LoadingFileInfo>();
				list.Add(new LoadingFileInfo(2, "image/map/20/show2", ""));
				base.Game.SendLoadResource(list);
			}
			else
			{
				base.Game.IsWin = false;
			}
		}

		protected int GetNpcCountByID(int Id)
		{
			int num = 0;
			foreach (SimpleNpc current in this.someNpc)
			{
				if (current.NpcInfo.ID == Id)
				{
					num++;
				}
			}
			return num;
		}
	}
}
