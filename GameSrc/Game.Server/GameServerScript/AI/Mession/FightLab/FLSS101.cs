using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class FLSS101 : AMissionControl
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

		private int int_6;

		private int int_7;

		private string[,] string_0;

		private string[] string_1;

		private int[] int_8;

		private int[] int_9;

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
			InitNpcCoor();
		}

		public void InitNpcCoor()
		{
			List<int> list = new List<int>();
			int num = -1;
			int num2 = 0;
			for (int i = 0; i < int_7; i++)
			{
				list.Clear();
				do
				{
					num2 = base.Game.Random.Next(0, 10);
					int_9[i] = (num2 + 1) * 100 + 75;
				}
				while (num2 == num);
				num = num2;
				int num3 = base.Game.Random.Next(0, 3);
				string_0[i, num3] = string_1[num2];
				int_8[i] = num3;
				if (num2 - 1 >= 0 && num2 + 1 < 10)
				{
					list.Add(num2 - 1);
					list.Add(num2 + 1);
				}
				else if (num2 - 1 < 0)
				{
					list.Add(num2 + 1);
					list.Add(num2 + 2);
				}
				else if (num2 + 1 >= 10)
				{
					list.Add(num2 - 1);
					list.Add(num2 - 2);
				}
				for (int j = 0; j < 3; j++)
				{
					if (string_0[i, j] != string_1[num2])
					{
						string_0[i, j] = string_1[list[0]];
						list.RemoveAt(0);
					}
				}
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
					Reset(isFirstNpc: true);
					KillNpc();
					break;
				case 2:
					method_0();
					break;
				case 3:
					if (bool_0)
					{
						Reset(isFirstNpc: true);
					}
					break;
				case 4:
					{
						if (!bool_0)
						{
							break;
						}
						int num = packet.ReadInt();
						int num2 = packet.ReadInt();
						if (num == int_4)
						{
							if (num2 == int_8[num - 1])
							{
								int_1++;
								method_0();
							}
							else
							{
								method_0();
							}
							int_5 = num;
							base.Game.WaitTime(0);
						}
						break;
					}
				case 5:
					if (bool_0)
					{
						Reset(isFirstNpc: false);
						method_0();
					}
					break;
			}
		}

		public void CreateNpc()
		{
			if (int_0 <= int_7)
			{
				if (bool_1)
				{
					simpleNpc_0 = base.Game.CreateNpc(int_6, 575, 505, 2);
					simpleNpc_0.SetRelateDemagemRect(-25, -55, 45, 55);
					bool_1 = false;
					int_0++;
				}
				else
				{
					simpleNpc_0 = base.Game.CreateNpc(int_6, int_9[int_4], 505, 2);
					simpleNpc_0.SetRelateDemagemRect(-25, -55, 45, 55);
					int_0++;
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

		public void Reset(bool isFirstNpc)
		{
			int_4 = 0;
			int_5 = 0;
			int_1 = 0;
			int_0 = 0;
			if (isFirstNpc)
			{
				bool_1 = true;
			}
			else
			{
				bool_1 = false;
			}
			bool_0 = false;
			string_0 = new string[10, 3]
			{
				{
					"",
					"",
					""
				},
				{
					"",
					"",
					""
				},
				{
					"",
					"",
					""
				},
				{
					"",
					"",
					""
				},
				{
					"",
					"",
					""
				},
				{
					"",
					"",
					""
				},
				{
					"",
					"",
					""
				},
				{
					"",
					"",
					""
				},
				{
					"",
					"",
					""
				},
				{
					"",
					"",
					""
				}
			};
			InitNpcCoor();
		}

		private void method_0()
		{
			if (int_4 <= int_7 - 1 && int_1 < int_2)
			{
				KillNpc();
				CreateNpc();
				bool_0 = true;
				base.Game.SendOpenPopupQuestionFrame(int_4 + 1, int_1, int_2, int_7, int_3, LanguageMgr.GetTranslation("FightLab.caption"), LanguageMgr.GetTranslation("FightLab.question"), string_0[int_4, 0], string_0[int_4, 1], string_0[int_4, 2]);
				int_4++;
			}
		}

		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			int[] npcIds = new int[1]
			{
				int_6
			};
			int[] npcIds2 = new int[3]
			{
				int_6,
				int_6,
				int_6
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.SetMap(1136);
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

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		public override bool CanGameOver()
		{
			if (int_1 >= int_2)
			{
				base.Game.SendClosePopupQuestionFrame();
				base.Game.WaitTime(1000);
				base.Game.IsWin = true;
				return true;
			}
			if (int_5 == int_7)
			{
				base.Game.SendClosePopupQuestionFrame();
				base.Game.WaitTime(1000);
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
			return base.Game.TotalKillCount;
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (int_1 >= int_2)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
			new List<LoadingFileInfo>();
		}

		public FLSS101()
		{

			bool_1 = true;
			int_2 = 3;
			int_3 = 30;
			int_6 = 4;
			int_7 = 10;
			string_0 = new string[10, 3]
			{
				{
					"",
					"",
					""
				},
				{
					"",
					"",
					""
				},
				{
					"",
					"",
					""
				},
				{
					"",
					"",
					""
				},
				{
					"",
					"",
					""
				},
				{
					"",
					"",
					""
				},
				{
					"",
					"",
					""
				},
				{
					"",
					"",
					""
				},
				{
					"",
					"",
					""
				},
				{
					"",
					"",
					""
				}
			};
			string_1 = new string[10]
			{
				"1",
				"2",
				"3",
				"4",
				"5",
				"6",
				"7",
				"8",
				"9",
				"10"
			};
			int_8 = new int[10];
			int_9 = new int[10];

		}
	}
}
