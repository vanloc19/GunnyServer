using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
	public class SeventhSimpleFirstBoss : ABrain
	{
		private int int_0;

		private bool bool_0;

		private List<PhysicalObj> list_0;

		private Player player_0;

		private static string[] string_0;

		private static string[] string_1;

		private static string[] string_2;

		private static string[] string_3;

		private static string[] string_4;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			if (bool_0)
			{
				base.Body.Config.HaveShield = true;
			}
			else
			{
				base.Body.Config.HaveShield = false;
			}
			base.Body.CurrentDamagePlus = 1f;
			base.Body.CurrentShootMinus = 1f;
			method_7();
		}

		public override void OnCreated()
		{
			base.OnCreated();
			base.Body.Properties1 = 0;
		}

		public override void OnDie()
		{
			base.OnDie();
			base.Body.Properties1 = 0;
			((SimpleBoss)base.Body).RandomSay(string_4, 0, 500, 2000);
		}

		public override void OnStartAttacking()
		{
			base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
			bool flag = false;
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				if (allFightPlayer.IsLiving && allFightPlayer.X > base.Body.X - 200 && allFightPlayer.X < base.Body.X + 200)
				{
					flag = true;
				}
			}
			if (flag)
			{
				method_8(base.Body.X - 200, base.Body.X + 200);
			}
			else if ((int)base.Body.Properties1 == 0 && !bool_0)
			{
				bool_0 = true;
				method_0();
			}
			else if ((int)base.Body.Properties1 == 1 && bool_0)
			{
				bool_0 = false;
				method_1(method_2);
			}
			else if ((int)base.Body.Properties1 == 1)
			{
				method_2();
			}
		}

		private void method_0()
		{
			base.Body.PlayMovie("toA", 1500, 3500);
		}

		private void method_1(LivingCallBack livingCallBack_0)
		{
			base.Body.PlayMovie("Ato", 1500, 3000);
			if (livingCallBack_0 != null)
			{
				base.Body.CallFuction(livingCallBack_0, 3500);
			}
		}

		private void method_2()
		{
			int_0++;
			switch (int_0)
			{
				case 1:
					method_3();
					break;
				case 2:
					method_4();
					break;
				case 3:
					method_5();
					int_0 = 0;
					break;
			}
			base.Body.Properties1 = 0;
		}

		private void method_3()
		{
			base.Body.CurrentDamagePlus = 2f;
			((SimpleBoss)base.Body).RandomSay(string_2, 0, 500, 0);
			base.Body.PlayMovie("beatB", 1000, 0);
			base.Body.CallFuction(method_6, 4000);
			base.Body.RangeAttacking(base.Body.X - 10000, base.Body.Y + 10000, "cry", 4500, null);
		}

		private void method_4()
		{
			player_0 = base.Game.FindRandomPlayer();
			((SimpleBoss)base.Body).RandomSay(string_0, 0, 500, 0);
			if (base.Body.ShootPoint(player_0.X, player_0.Y, 84, 1200, 10000, 1, 3f, 2000))
			{
				base.Body.PlayMovie("beat", 1000, 0);
			}
		}

		private void method_5()
		{
			player_0 = base.Game.FindRandomPlayer();
			((SimpleBoss)base.Body).RandomSay(string_1, 0, 500, 0);
			if (base.Body.ShootPoint(player_0.X, player_0.Y, 84, 1200, 10000, 2, 3f, 2000))
			{
				base.Body.PlayMovie("beatA", 1000, 0);
			}
		}

		private void method_6()
		{
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				list_0.Add(((PVEGame)base.Game).Createlayer(allLivingPlayer.X, allLivingPlayer.Y, "moive", "asset.game.seven.cao", "in", 1, 0));
			}
		}

		private void method_7()
		{
			foreach (PhysicalObj item in list_0)
			{
				if (item != null)
				{
					base.Game.RemovePhysicalObj(item, sendToClient: true);
				}
			}
			list_0 = new List<PhysicalObj>();
		}

		private void method_8(int int_1, int int_2)
		{
			base.Body.CurrentDamagePlus = 1000f;
			((SimpleBoss)base.Body).RandomSay(string_3, 0, 500, 2000);
			base.Body.PlayMovie("beatB", 1000, 0);
			base.Body.RangeAttacking(int_1, int_2, "cry", 3000, null);
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		public SeventhSimpleFirstBoss()
		{

			list_0 = new List<PhysicalObj>();

		}

		static SeventhSimpleFirstBoss()
		{

			string_0 = new string[3]
			{
				"Bắn trứng siêu cấp!",
				"Ta ị vào mặt mi!!!!",
				"Đỡ tuyệt chiêu tào tháo của ta."
			};
			string_1 = new string[3]
			{
				"Ta ném ngươi thối mặt",
				"Trứng thối tới đây!",
				"Cái này vô mặt phê lắm nha."
			};
			string_2 = new string[3]
			{
				"Ta cào rách mặt ngươi",
				"Cào!!! Cào!!!",
				"Đỡ chiêu gà bới này."
			};
			string_3 = new string[2]
			{
				"Tới đây làm gì?",
				"Ngu kinh. Ngu éo tả."
			};
			string_4 = new string[5]
			{
				"Thôi thôi té gấp",
				"Dữ vãi chuồn thôi",
				"Tụi nó mạnh quá em chuồn trước nhé",
				"Mấy chụy ở lại mạnh khỏe e chuồn.",
				"Chết, quá dữ. Chuồn gấp!!!!"
			};
		}
	}
}
