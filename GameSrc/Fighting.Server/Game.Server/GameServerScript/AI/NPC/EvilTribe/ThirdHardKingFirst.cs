using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class ThirdHardKingFirst : ABrain
	{
		private int int_0;

		private List<PhysicalObj> list_0;

		private SimpleBoss simpleBoss_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private int int_4;

		private static string[] string_0;

		private static string[] string_1;

		private static string[] string_2;

		private static string[] string_3;

		private static string[] string_4;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
			if (simpleBoss_0 != null && simpleBoss_0.IsLiving)
			{
				return;
			}
			foreach (Living item in base.Game.FindAllTurnBossLiving())
			{
				if (item != base.Body)
				{
					simpleBoss_0 = item as SimpleBoss;
					break;
				}
			}
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			m_body.CurrentDamagePlus = 1f;
			m_body.CurrentShootMinus = 1f;
		}

		public override void OnCreated()
		{
			base.OnCreated();
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
			bool flag = false;
			int num = 0;
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				if (allFightPlayer.IsLiving && allFightPlayer.X > base.Body.X - 300 && allFightPlayer.X < base.Body.X + 300)
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
				method_0(base.Body.X - 300, base.Body.X + 300);
			}
			else if (simpleBoss_0 != null && !simpleBoss_0.IsLiving)
			{
				base.Body.PlayMovie("castA", 800, 0);
				base.Body.Say("Em trai ơi đừng đầu hàng.<br/>Anh sẽ giúp em hồi sinh!!!", 0, 1000);
				((PVEGame)base.Game).SendObjectFocus(simpleBoss_0, 1, 3000, 1000);
				base.Body.CallFuction(ReviveFriendBoss, 5000);
			}
			else if (simpleBoss_0 != null && simpleBoss_0.Blood <= simpleBoss_0.NpcInfo.Blood / 10)
			{
				base.Body.PlayMovie("castA", 800, 0);
				base.Body.Say("Các ngươi làm sao đủ khả năng đánh bại em ta!!!", 0, 1000);
				((PVEGame)base.Game).SendObjectFocus(simpleBoss_0, 1, 3500, 5000);
				base.Body.CallFuction(RecoverFriendBlood, 4000);
			}
			else
			{
				method_1();
			}
		}

		public void RecoverFriendBlood()
		{
			simpleBoss_0.AddBlood(int_4);
		}

		public void ReviveFriendBoss()
		{
			SimpleBoss simpleBoss = null;
			LivingConfig livingConfig = ((PVEGame)base.Game).BaseLivingConfig();
			livingConfig.ReduceBloodStart = 2;
			base.Game.RemoveLiving(simpleBoss_0.Id);
			simpleBoss = ((PVEGame)base.Game).CreateBoss(simpleBoss_0.NpcInfo.ID, simpleBoss_0.X, simpleBoss_0.Y, simpleBoss_0.Direction, 1, "raise", livingConfig);
			simpleBoss.SetRelateDemagemRect(simpleBoss.NpcInfo.X, simpleBoss.NpcInfo.Y, simpleBoss.NpcInfo.Width, simpleBoss.NpcInfo.Height);
			simpleBoss.Say("Ta đã hồi sinh!!! Các ngươi phải chết.", 0, 3000);
			simpleBoss_0 = simpleBoss;
		}

		private void method_0(int int_5, int int_6)
		{
			base.Body.CurrentDamagePlus = 100f;
			base.Body.PlayMovie("call", 1000, 0);
			base.Body.RangeAttacking(int_5, int_6, "cry", 4000, null);
		}

		private void method_1()
		{
			if (base.Body.Y >= 310 && base.Body.Y <= 420)
			{
				base.Body.FallFromTo(base.Body.X, 483, "", 1000, 0, 25, method_2);
			}
			else if (base.Body.Y >= 450 && base.Body.Y <= 540)
			{
				base.Body.PlayMovie("walk", 100, 1000);
				base.Body.FallFromTo(base.Body.X, 623, "", 1000, 0, 25, method_2);
			}
			else if (base.Body.Y >= 580 && base.Body.Y <= 668)
			{
				base.Body.JumpTo(base.Body.X, 360, "walk", 1000, 1, 20, method_2);
			}
		}

		private void method_2()
		{
			switch (int_0)
			{
				case 0:
					if (!method_3())
					{
						method_6();
					}
					else
					{
						method_9();
					}
					int_0++;
					break;
				case 1:
					if (!method_4())
					{
						method_8();
					}
					else
					{
						method_9();
					}
					int_0++;
					break;
				case 2:
					method_9();
					int_0++;
					break;
				case 3:
					if (!method_5())
					{
						method_7();
					}
					else
					{
						method_9();
					}
					int_0 = 0;
					break;
			}
		}

		private bool method_3()
		{
			SimpleNpc[] array = base.Game.FindAllNpc();
			int num = 0;
			while (true)
			{
				if (num < array.Length)
				{
					SimpleNpc simpleNpc = array[num];
					if (simpleNpc.IsLiving && simpleNpc.NpcInfo.ID == int_2)
					{
						break;
					}
					num++;
					continue;
				}
				return false;
			}
			return true;
		}

		private bool method_4()
		{
			SimpleNpc[] array = base.Game.FindAllNpc();
			int num = 0;
			while (true)
			{
				if (num < array.Length)
				{
					SimpleNpc simpleNpc = array[num];
					if (simpleNpc.IsLiving && simpleNpc.NpcInfo.ID == int_1)
					{
						break;
					}
					num++;
					continue;
				}
				return false;
			}
			return true;
		}

		private bool method_5()
		{
			SimpleNpc[] array = base.Game.FindAllNpc();
			int num = 0;
			while (true)
			{
				if (num < array.Length)
				{
					SimpleNpc simpleNpc = array[num];
					if (simpleNpc.IsLiving && simpleNpc.NpcInfo.ID == int_3)
					{
						break;
					}
					num++;
					continue;
				}
				return false;
			}
			return true;
		}

		private void method_6()
		{
			int num = base.Game.Random.Next(0, string_2.Length);
			base.Body.Say(string_2[num], 0, 2000);
			base.Body.PlayMovie("call", 2000, 0);
			base.Body.CallFuction(CreateFearNPC, 5000);
		}

		public void CreateFearNPC()
		{
			((SimpleBoss)base.Body).CreateChild(int_2, 1079, 631, 0, 3, -1);
		}

		private void method_7()
		{
			int num = base.Game.Random.Next(0, string_3.Length);
			base.Body.Say(string_3[num], 0, 2000);
			base.Body.PlayMovie("call", 2000, 0);
			base.Body.CallFuction(CreateLockNPC, 5000);
		}

		public void CreateLockNPC()
		{
			((SimpleBoss)base.Body).CreateChild(int_3, 567, 631, 0, 3, 1);
		}

		private void method_8()
		{
			int num = base.Game.Random.Next(0, string_1.Length);
			base.Body.Say(string_1[num], 0, 2300);
			base.Body.PlayMovie("castA", 2500, 0);
			base.Body.CallFuction(CreateChild, 5000);
		}

		public void CreateChild()
		{
			((SimpleBoss)base.Body).CreateChild(int_1, 217, 242, 0, 3, 1);
		}

		private void method_9()
		{
			Player player = base.Game.FindRandomPlayer();
			base.Body.PlayMovie("stand", 0, 0);
			base.Body.CurrentDamagePlus = 1.8f;
			if (player != null && base.Body.ShootPoint(player.X, player.Y, 54, 1000, 10000, 3, 2f, 2300))
			{
				base.Body.PlayMovie("beatA", 1500, 0);
			}
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		public ThirdHardKingFirst()
		{
			list_0 = new List<PhysicalObj>();
			int_1 = 3206;
			int_2 = 3210;
			int_3 = 3211;
			int_4 = 5000;
		}

		static ThirdHardKingFirst()
		{
			string_0 = new string[3]
			{
				"Trận động đất, bản thân mình! ! <br/> bạn vui lòng Ay giúp đỡ",
				"Hạ vũ khí xuống!",
				"Xem nếu bạn có thể đủ khả năng, một số ít!！"
			};
			string_1 = new string[4]
			{
				"Vệ binh! <br/> bảo vệ! ! ",
				"Dũng sĩ bộ lạc đâu thể hiện đi!",
				"Đệ tử của ta đâu mau ra đây!!!",
				"Lính đâu ra giết bọn chúng."
			};
			string_2 = new string[3]
			{
				"Có cảm thấy bủn rủn không?",
				"Ngươi không đủ sức để hạ ta đâu.",
				"Thấy mệt mỏi chưa?"
			};
			string_3 = new string[2]
			{
				"Sao mà bắn ta được?",
				"Giỏi thì thử bắn nữa đi."
			};
			string_4 = new string[2]
			{
				"Sao mà bắn ta được?",
				"Giỏi thì thử bắn nữa đi."
			};
		}
	}
}
