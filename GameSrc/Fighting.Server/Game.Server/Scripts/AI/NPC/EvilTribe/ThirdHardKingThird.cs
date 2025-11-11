using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class ThirdHardKingThird : ABrain
	{
		private int int_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private int int_4;

		private int int_5;

		public List<SimpleNpc> orchins;

		private static string[] string_0;

		private static string[] string_1;

		private static string[] string_2;

		private static string[] string_3;

		private static string[] string_4;

		private static string[] string_5;

		private static string[] string_6;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
			if (int_5 > 0)
			{
				int_5--;
			}
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			base.Body.CurrentDamagePlus = 1f;
			base.Body.CurrentShootMinus = 1f;
		}

		public override void OnCreated()
		{
			base.OnCreated();
		}

		public override void OnStartAttacking()
		{
			bool flag = false;
			int num = 0;
			base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				if (allFightPlayer.IsLiving && allFightPlayer.X > base.Body.X - 250 && allFightPlayer.X < base.Body.X + 250)
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
				KillAttack(base.Body.X - 250, base.Body.X + 250);
				return;
			}
			if (int_0 == 1)
			{
				HalfAttack();
			}
			else if (int_0 == 2)
			{
				method_1();
			}
			else if (int_0 == 3)
			{
				List<SimpleNpc> list = (base.Body as SimpleBoss).FindChildLiving(int_3);
				if (list.Count <= 0)
				{
					Summon();
				}
				else
				{
					(base.Body as SimpleBoss).FindChildLiving(int_2);
					if (list.Count <= 0)
					{
						SummonExplode();
					}
					else
					{
						method_1();
					}
				}
			}
			else if (int_0 == 4)
			{
				method_0();
			}
			else if (int_0 == 5)
			{
				if (base.Game.Random.Next(0, 10) > 5)
				{
					SummonExplode();
				}
				else
				{
					method_1();
				}
			}
			else
			{
				if ((base.Body as SimpleBoss).FindChildLiving(int_1).Count <= 0)
				{
					SummonNpc();
				}
				else
				{
					method_0();
				}
				int_0 = 0;
			}
			int_0++;
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
			base.Game.RemoveLiving(int_1);
		}

		public void HalfAttack()
		{
			base.Body.CurrentDamagePlus = 1.5f;
			int num = base.Game.Random.Next(0, string_0.Length);
			base.Body.Say(string_0[num], 1, 500);
			base.Body.PlayMovie("beatC", 2500, 0);
			base.Body.RangeAttacking(base.Body.X - 10000, base.Body.Y + 10000, "cry", 3500, null);
		}

		private void method_0()
		{
			Player[] allPlayers = base.Game.GetAllPlayers();
			int num = 200;
			int num2 = base.Game.Random.Next(0, string_1.Length);
			base.Body.Say(string_1[num2], 1, 200);
			Player[] array = allPlayers;
			Player[] array2 = array;
			foreach (Player player in array2)
			{
				if (player.IsLiving)
				{
					num += 200;
					base.Body.ChangeDirection(base.Body.FindDirection(player), num);
					num += 2000;
					if (base.Body.ShootPoint(player.X, player.Y, 55, 1000, 10000, 1, 1.5f, num))
					{
						base.Body.PlayMovie("beatB", num - 1000, 0);
					}
				}
			}
		}

		private void method_1()
		{
			int x = base.Game.Random.Next(700, 800);
			base.Body.MoveTo(x, base.Body.Y, "walk", 1000, "", 4, method_2);
		}

		private void method_2()
		{
			Player[] allPlayers = base.Game.GetAllPlayers();
			int num = 200;
			int num2 = base.Game.Random.Next(0, string_1.Length);
			base.Body.Say(string_1[num2], 1, 200);
			Player[] array = allPlayers;
			Player[] array2 = array;
			foreach (Player player in array2)
			{
				if (player.IsLiving)
				{
					num += 200;
					base.Body.ChangeDirection(base.Body.FindDirection(player), num);
					num += 2000;
					if (base.Body.ShootPoint(player.X, player.Y, 54, 1000, 10000, 1, 1.5f, num))
					{
						base.Body.PlayMovie("beatA", num - 1000, 0);
					}
				}
			}
		}

		public void Summon()
		{
			int_5 = 5;
			int num = base.Game.Random.Next(0, string_3.Length);
			base.Body.Say(string_3[num], 1, 0);
			base.Body.PlayMovie("callA", 100, 0);
			base.Body.CallFuction(CreateBuffBlood, 2500);
		}

		public void SummonNpc()
		{
			int num = base.Game.Random.Next(0, string_2.Length);
			base.Body.Say(string_2[num], 1, 0);
			base.Body.PlayMovie("callB", 100, 0);
			base.Body.CallFuction(CreateChild, 4500);
		}

		public void SummonExplode()
		{
			int num = base.Game.Random.Next(0, string_4.Length);
			base.Body.Say(string_4[num], 1, 0);
			base.Body.PlayMovie("callA", 100, 0);
			base.Body.CallFuction(CreateExplode, 2500);
		}

		public void KillAttack(int fx, int mx)
		{
			base.Body.CurrentDamagePlus = 1000f;
			int num = base.Game.Random.Next(0, string_6.Length);
			((SimpleBoss)base.Body).Say(string_6[num], 1, 500);
			base.Body.PlayMovie("beatC", 2500, 0);
			base.Body.RangeAttacking(fx, mx, "cry", 3300, null);
		}

		public void CreateChild()
		{
			int num = 1000;
			for (int i = 0; i < int_4; i++)
			{
				((SimpleBoss)base.Body).CreateChild(int_1, num, 555, -1, int_4, -1);
				num -= 50;
			}
		}

		public void CreateBuffBlood()
		{
			((SimpleBoss)base.Body).CreateBoss(int_3, 568, 554, -1, 0);
			((SimpleBoss)base.Body).CreateBoss(int_3, 1093, 556, 1, 0);
		}

		public void CreateExplode()
		{
			Player player = base.Game.FindRandomPlayer();
			if (player != null)
			{
				((SimpleBoss)base.Body).CreateBoss(int_2, player.X, player.Y, player.Direction, 0);
			}
		}

		public ThirdHardKingThird()
		{
			int_0 = 1;
			int_1 = 3203;
			int_2 = 3212;
			int_3 = 3213;
			int_4 = 15;
			orchins = new List<SimpleNpc>();
		}

		static ThirdHardKingThird()
		{
			string_0 = new string[1]
			{
				"Tiếng gầm của mảnh hổ !!..."
			};
			string_1 = new string[4]
			{
				"Nhìn có giống cái gì không?",
				"Ta ném!! Tá ném!!!!",
				"Giỏi thì né nữa đi.",
				"Bách phát bách trúng"
			};
			string_2 = new string[1]
			{
				"Vũ điệu<br/>săn bắn...."
			};
			string_3 = new string[3]
			{
				"Sức mạnh bộ tộc<br/>Hãy giúp ta!!!",
				"Ta muốn có nhiều sức mạnh!!!!",
				"Tiếp sức cho ta tiêu diệt kẻ thù..."
			};
			string_4 = new string[3]
			{
				"Có đỡ được không?",
				"Bộ lạc hãy giúp ta",
				"Tiếp sức!!! Tiếp sức!!!!"
			};
			string_5 = new string[3]
			{
				"Dám chọc giận ta à?",
				"Ta né, ta né!!!",
				"Đồ khốn nạn. Dám đánh ta!!"
			};
			string_6 = new string[1]
			{
				"Muốn xem lợi hại của cổ họng của ta!"
			};
		}
	}
}
