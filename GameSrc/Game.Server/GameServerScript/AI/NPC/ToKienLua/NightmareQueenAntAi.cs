using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic;
using System.Drawing;
using Bussiness;


namespace GameServerScript.AI.NPC
{
    public class NightmareQueenAntAi : ABrain
	{
		private int int_0;

		private int int_1;

		private int int_2;

		private Point[] point_0;

		private static string[] string_0;

		private static string[] string_1;

		private static string[] string_2;

		private static string[] string_3;

		private static string[] string_4;

		private static string[] string_5;

		private static string[] string_6;

		private static string[] string_7;

		Point[] brithPoint = new Point[]
		{
			new Point(979, 630),
			new Point(1013, 630),
			new Point(1052, 630),
			new Point(1088, 630),
			new Point(1142, 630)
		};

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			base.Body.CurrentDamagePlus = 1f;
			base.Body.CurrentShootMinus = 1f;
			int_2 = 0;
		}

		public override void OnCreated()
		{
			base.OnCreated();
		}

		public override void OnStartAttacking()
		{
			base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
			bool flag = false;
			int num = 0;
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				if (allFightPlayer.IsLiving && allFightPlayer.X > 1169 && allFightPlayer.X < base.Game.Map.Info.ForegroundWidth + 1)
				{
					int num2 = (int)base.Body.Distance(allFightPlayer.X, allFightPlayer.Y);
					if (num2 > num)
					{
						num = num2;
					}
					flag = true;
				}
			}
			if (flag)
			{
				method_0(1169, base.Game.Map.Info.ForegroundWidth + 1);
			}
			else if (int_0 == 0)
			{
				if (((PVEGame)base.Game).GetLivedLivings().Count == 9)
				{
					method_1();
				}
				else
				{
					method_2();
				}
				int_0++;
			}
			else
			{
				method_1();
				int_0 = 0;
			}
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		private void method_0(int int_3, int int_4)
		{
			int num = base.Game.Random.Next(0, string_5.Length);
			base.Body.Say(string_5[num], 1, 1000);
			base.Body.CurrentDamagePlus = 100f;
			base.Body.PlayMovie("beatB", 3000, 0);
			base.Body.RangeAttacking(int_3, int_4, "cry", 5000, null);
		}

		private void method_1()
		{
			Player player = base.Game.FindRandomPlayer();
			if (player != null)
			{
				base.Body.CurrentDamagePlus = 1.8f;
				int num = base.Game.Random.Next(0, string_1.Length);
				base.Body.Say(string_1[num], 1, 0);
				base.Game.Random.Next(670, 880);
				base.Game.Random.Next(player.X - 10, player.X + 10);
				if (base.Body.ShootPoint(player.X, player.Y, 51, 1000, 10000, 1, 3f, 2550))
				{
					base.Body.PlayMovie("beatA", 1700, 0);
				}
			}
		}

		private void method_2()
		{
			int num = base.Game.Random.Next(0, string_3.Length);
			base.Body.Say(string_3[num], 1, 600);
			base.Body.PlayMovie("call", 1700, 2000);
			base.Body.CallFuction(method_3, 2000);
		}

		private void method_3()
		{
			((SimpleBoss)base.Body).CreateChild(this.int_1, this.brithPoint, 9, 3, 0);
		}

		public new void OnKillPlayerSay()
		{
			int num = base.Game.Random.Next(0, string_2.Length);
			base.Body.Say(string_2[num], 1, 0, 2000);
		}

		public override void OnDiedSay()
		{
		}

		private void method_4()
		{
		}

		public void OnShootedSay(int delay)
		{
			int num = base.Game.Random.Next(0, string_6.Length);
			if (int_2 == 0 && base.Body.IsLiving)
			{
				base.Body.Say(string_6[num], 1, delay, 0);
				int_2 = 1;
			}
			if (!base.Body.IsLiving)
			{
				num = base.Game.Random.Next(0, string_7.Length);
				base.Body.Say(string_7[num], 1, delay - 800, 2000);
			}
		}

		public NightmareQueenAntAi()
		{

			int_1 = 2404;
			point_0 = new Point[5]
			{
				new Point(979, 630),
				new Point(1013, 630),
				new Point(1052, 630),
				new Point(1088, 630),
				new Point(1142, 630)
			};

		}

		static NightmareQueenAntAi()
		{

			string_0 = new string[3]
			{
				LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg1"),
				LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg2"),
				LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg3")
			};
			string_1 = new string[2]
			{
				LanguageMgr.GetTranslation("Mũi tên với độ chính xác tuyệt vời!"),
				LanguageMgr.GetTranslation("Cho bạn nếm mùi lợi hại!")
			};
			string_2 = new string[2]
			{
				LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg6"),
				LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg7")
			};
			string_3 = new string[2]
			{
				LanguageMgr.GetTranslation("Hỡi những nô lệ tiêu diệt kể xâm nhập!"),
				LanguageMgr.GetTranslation("Hộ giá, hộ giá!")
			};
			string_4 = new string[3]
			{
				LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg10"),
				LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg11"),
				LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg12")
			};
			string_5 = new string[2]
			{
				LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg13"),
				LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg14")
			};
			string_6 = new string[2]
			{
				LanguageMgr.GetTranslation("Trúng phóc"),
				LanguageMgr.GetTranslation("Cho nếm mùi lợi hại !")
			};
			string_7 = new string[1]
			{
				LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg17")
			};
		}
	}
}
