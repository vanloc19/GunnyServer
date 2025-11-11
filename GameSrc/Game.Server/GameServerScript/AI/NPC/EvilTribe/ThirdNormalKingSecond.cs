using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class ThirdNormalKingSecond : ABrain
	{
		private int int_0;

		private int int_1;

		private SimpleBoss simpleBoss_0;

		private int int_2;

		private List<PhysicalObj> list_0;

		private static string[] string_0;

		private static string[] string_1;

		private static string[] string_2;

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
				method_5(base.Body.X - 300, base.Body.X + 300);
			}
			else if (simpleBoss_0 != null && !simpleBoss_0.IsLiving)
			{
				base.Body.PlayMovie("castA", 800, 0);
				base.Body.Say("Anh trai ơi đừng đầu hàng.<br/>Em sẽ giúp anh hồi sinh!!!", 0, 1000);
				((PVEGame)base.Game).SendObjectFocus(simpleBoss_0, 1, 3000, 1000);
				base.Body.CallFuction(ReviveFriendBoss, 5000);
			}
			else if (simpleBoss_0 != null && simpleBoss_0.Blood <= simpleBoss_0.NpcInfo.Blood / 10)
			{
				base.Body.PlayMovie("castA", 800, 0);
				base.Body.Say("Các ngươi làm sao đủ khả năng đánh bại anh ta!!!", 0, 1000);
				((PVEGame)base.Game).SendObjectFocus(simpleBoss_0, 1, 3500, 5000);
				base.Body.CallFuction(RecoverFriendBlood, 4000);
			}
			else
			{
				method_0();
			}
		}

		public void RecoverFriendBlood()
		{
			simpleBoss_0.AddBlood(int_2);
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

		private void method_0()
		{
			if (base.Body.Y >= 310 && base.Body.Y <= 420)
			{
				base.Body.FallFromTo(base.Body.X, 477, "", 1000, 0, 25, method_1);
			}
			else if (base.Body.Y >= 450 && base.Body.Y <= 540)
			{
				base.Body.PlayMovie("walk", 100, 1000);
				base.Body.FallFromTo(base.Body.X, 623, "", 1000, 0, 25, method_1);
			}
			else if (base.Body.Y >= 580 && base.Body.Y <= 668)
			{
				base.Body.JumpTo(base.Body.X, 361, "walk", 1000, 1, 20, method_1);
			}
		}

		private void method_1()
		{
			switch (int_0)
			{
				case 0:
					method_2();
					break;
				case 1:
					method_4();
					break;
				case 2:
					method_3();
					int_0 = 0;
					break;
			}
			int_0++;
		}

		private void method_2()
		{
			Player player = base.Game.FindRandomPlayer();
			base.Body.PlayMovie("stand", 0, 0);
			base.Body.CurrentDamagePlus = 1.8f;
			int num = base.Game.Random.Next(0, string_2.Length);
			base.Body.Say(string_2[num], 0, 1000);
			if (player != null && base.Body.ShootPoint(player.X, player.Y, 54, 1000, 10000, 3, 2f, 2300))
			{
				base.Body.PlayMovie("beatA", 1500, 0);
			}
		}

		private void method_3()
		{
			int num = base.Game.Random.Next(0, string_1.Length);
			base.Body.Say(string_1[num], 0, 1000);
			base.Body.PlayMovie("castA", 1000, 0);
			base.Body.CallFuction(CreateChild, 4000);
		}

		public void CreateChild()
		{
			((SimpleBoss)base.Body).CreateChild(int_1, 1369, 242, 0, 1, -1);
		}

		private void method_4()
		{
			base.Body.CurrentDamagePlus = 1.5f;
			int num = base.Game.Random.Next(0, string_0.Length);
			base.Body.Say(string_0[num], 0, 1000);
			base.Body.PlayMovie("castA", 1000, 0);
			Player obj = base.Game.FindRandomPlayer();
			((PVEGame)base.Game).SendObjectFocus(obj, 2, 3000, 1000);
			base.Body.CallFuction(CreateStarMovie, 4000);
		}

		public void CreateStarMovie()
		{
			Player[] allPlayers = base.Game.GetAllPlayers();
			Player[] array = allPlayers;
			foreach (Player player in array)
			{
				if (player.IsLiving)
				{
					PhysicalObj item = ((PVEGame)base.Game).Createlayer(player.X, player.Y, "", "game.crazytank.assetmap.Dici", "", 1, 0);
					list_0.Add(item);
				}
			}
			base.Body.RangeAttacking(-10000, 10000, "cry", 700, null);
			base.Body.CallFuction(RemoveMovie, 1500);
		}

		public void RemoveMovie()
		{
			foreach (PhysicalObj item in list_0)
			{
				base.Game.RemovePhysicalObj(item, sendToClient: true);
			}
			list_0 = new List<PhysicalObj>();
		}

		private void method_5(int int_3, int int_4)
		{
			base.Body.CurrentDamagePlus = 100f;
			base.Body.PlayMovie("call", 1000, 0);
			base.Body.RangeAttacking(int_3, int_4, "cry", 4000, null);
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		public ThirdNormalKingSecond()
		{
			int_1 = 3106;
			int_2 = 2000;
			list_0 = new List<PhysicalObj>();
		}

		static ThirdNormalKingSecond()
		{
			string_0 = new string[4]
			{
				"Kiếm thần!!! Kiếm thần!!",
				"Hãy đỡ tuyệt chiêu kiếm thần!!",
				"Kiếm thần!! Trợ giúp!!",
				"Kiếm thần đây. Hãy đỡ!!!!"
			};
			string_1 = new string[4]
			{
				"Vệ binh! <br/> bảo vệ! ! ",
				"Dũng sĩ bộ lạc đâu thể hiện đi!",
				"Đệ tử của ta đâu mau ra đây!!!",
				"Lính đâu ra giết bọn chúng."
			};
			string_2 = new string[4]
			{
				"Chạy à? Chạy nữa đi!!!",
				"Một phát chết 3 con gián.",
				"Trẻ trâu mà láo à?",
				"Lũ gà!!! Chết này."
			};
		}
	}
}
