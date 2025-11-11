using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Actions;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class FLSS104 : AMissionControl
	{
		private SimpleNpc simpleNpc_0;

		private bool bool_0;

		private bool bool_1;

		private int int_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private int int_4;

		private int int_5;

		private int[] int_6;

		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			if (score > 1870)
			{
				return 3;
			}
			if (score > 1825)
			{
				return 2;
			}
			if (score > 1780)
			{
				return 1;
			}
			return 0;
		}

		public override void OnPrepareNewGame()
		{
			base.OnPrepareNewGame();
			base.Game.FrozenWind = true;
			int num = -1;
			int num2 = 0;
			for (int i = 0; i < int_5; i++)
			{
				do
				{
					num2 = base.Game.Random.Next(5, 20);
					int_6[i] = (num2 + 1) * 100 + 75;
				}
				while (num2 == num);
				num = num2;
			}
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			if (bool_0)
			{
				KillNpc();
				CreateNpc();
			}
		}

		public override void OnGeneralCommand(GSPacketIn packet)
		{
			switch (packet.ReadInt())
			{
				case 0:
					CreateNpc();
					break;
				case 1:
					bool_0 = false;
					Reset();
					KillNpc();
					break;
				case 2:
					bool_0 = true;
					simpleNpc_0 = null;
					base.Game.AddAction(new LivingCallFunctionAction(null, Skip, 4000));
					break;
			}
		}

		public void Skip()
		{
			base.Game.CurrentPlayer.Skip(0);
		}

		public void CreateNpc()
		{
			if (int_0 <= int_5)
			{
				if (bool_1)
				{
					simpleNpc_0 = base.Game.CreateNpc(int_4, 1075, 565, 2, -1, base.Game.BaseLivingConfig());
					simpleNpc_0.Config.IsTurn = false;
					bool_1 = false;
					base.Game.WaitTime(0);
				}
				else
				{
					simpleNpc_0 = base.Game.CreateNpc(int_4, int_6[int_3], 565, 2, -1, base.Game.BaseLivingConfig());
					simpleNpc_0.Config.IsTurn = false;
					int_0++;
					int_3++;
					base.Game.WaitTime(0);
				}
			}
		}

		public void KillNpc()
		{
			if (simpleNpc_0 != null && simpleNpc_0.IsLiving)
			{
				base.Game.RemoveLiving(simpleNpc_0, true);
				simpleNpc_0 = null;
			}
		}

		public void Reset()
		{
			int_3 = 0;
			int_1 = 0;
			int_0 = 0;
			bool_1 = true;
			bool_0 = false;
		}

		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			int[] npcIds = new int[1]
			{
				int_4
			};
			int[] npcIds2 = new int[3]
			{
				int_4,
				int_4,
				int_4
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.SetMap(1135);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
			if (base.Game.CurrentLiving != null)
			{
				((Player)base.Game.CurrentLiving).Seal((Player)base.Game.CurrentLiving, 0, 0);
			}
		}

		public override bool CanGameOver()
		{
			if (bool_0 && simpleNpc_0 != null && !simpleNpc_0.IsLiving)
			{
				int_1++;
			}
			if (int_1 >= int_2)
			{
				base.Game.IsWin = true;
				return true;
			}
			if (int_0 == int_5)
			{
				if (int_1 >= int_2)
				{
					base.Game.IsWin = true;
				}
				else
				{
					base.Game.IsWin = false;
				}
				return true;
			}
			return false;
		}

		public override int UpdateUIData()
		{
			base.Game.Param1 = int_0;
			return int_1;
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			new List<LoadingFileInfo>();
		}

		public FLSS104()
		{

			bool_1 = true;
			int_2 = 3;
			int_4 = 4;
			int_5 = 10;
			int_6 = new int[10];

		}
	}
}
