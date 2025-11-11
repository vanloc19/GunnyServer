using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Actions;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;
using System.Drawing;

namespace GameServerScript.AI.Messions
{
	public class FLNS115 : AMissionControl
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

		private Point[] point_0;

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
				LivingConfig livingConfig = base.Game.BaseLivingConfig();
				livingConfig.IsTurn = false;
				livingConfig.IsFly = true;
				if (bool_1)
				{
					simpleNpc_0 = base.Game.CreateNpc(int_4, 932, 769, 2, 1, livingConfig);
					bool_1 = false;
					base.Game.WaitTime(0);
				}
				else
				{
					int num = base.Game.Random.Next(0, point_0.Length);
					simpleNpc_0 = base.Game.CreateNpc(int_4, point_0[num].X, point_0[num].Y, 2, 1, livingConfig);
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
			base.Game.SetMap(1138);
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

		public FLNS115()
		{

			int_2 = 6;
			int_4 = 6;
			int_5 = 10;
			point_0 = new Point[18]
			{
				new Point(77, 573),
				new Point(109, 573),
				new Point(141, 573),
				new Point(173, 573),
				new Point(205, 573),
				new Point(237, 573),
				new Point(442, 1271),
				new Point(479, 1271),
				new Point(516, 1271),
				new Point(553, 1271),
				new Point(590, 1271),
				new Point(627, 1271),
				new Point(866, 769),
				new Point(899, 769),
				new Point(932, 769),
				new Point(965, 769),
				new Point(998, 769),
				new Point(1031, 769)
			};

		}
	}
}
