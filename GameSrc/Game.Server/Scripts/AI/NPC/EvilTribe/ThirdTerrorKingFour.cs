using System.Collections.Generic;
using System.Drawing;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class ThirdTerrorKingFour : ABrain
	{
		private bool bool_0;

		private int int_0;

		private int int_1;

		private int int_2;

		private SimpleNpc simpleNpc_0;

		private int int_3;

		private List<PhysicalObj> list_0;

		private List<Point> list_1;

		private int int_4;

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
			if (int_4 > 0)
			{
				int_4--;
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
			if (!bool_0)
			{
				method_5();
				bool_0 = true;
				return;
			}
			bool flag = false;
			base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				if (allFightPlayer.IsLiving && allFightPlayer.X > base.Body.X - 200 && allFightPlayer.X < base.Body.X + 200)
				{
					flag = true;
				}
			}
			if (flag)
			{
				KillAttack(base.Body.X - 200, base.Body.X + 200);
				return;
			}
			if (int_4 <= 0)
			{
				int_4 = 5;
				method_4();
				return;
			}
			switch (int_3)
			{
				case 0:
					method_1();
					break;
				case 1:
					method_0();
					break;
				case 2:
					method_1();
					break;
				case 3:
					method_2();
					break;
				case 4:
					method_1();
					int_3 = 0;
					break;
			}
			int_3++;
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		private void method_0()
		{
			base.Body.CurrentDamagePlus = 1.5f;
			int num = base.Game.Random.Next(0, string_0.Length);
			base.Body.Say(string_0[num], 1, 500);
			base.Body.PlayMovie("beatB", 1000, 3000);
			base.Body.RangeAttacking(base.Body.X - 10000, base.Body.Y + 10000, "cry", 3000, null);
		}

		private void method_1()
		{
			int num = base.Game.Random.Next(0, string_1.Length);
			base.Body.Say(string_1[num], 1, 500);
			Player player = base.Game.FindRandomPlayer();
			if (player == null)
			{
				return;
			}
			base.Body.ChangeDirection(base.Body.FindDirection(player), 100);
			int num2 = 2000;
			for (int i = 0; i < int_1; i++)
			{
				if (base.Body.ShootPoint(player.X, player.Y, 53, 1000, 10000, 3, 2.3f, num2))
				{
					base.Body.PlayMovie("aim", num2 - 1000, 0);
					base.Body.PlayMovie("beatA", num2 - 500, 0);
				}
				num2 += 2000;
			}
		}

		private void method_2()
		{
			base.Body.CurrentDamagePlus = 2f;
			int num = base.Game.Random.Next(0, string_5.Length);
			((SimpleBoss)base.Body).Say(string_5[num], 1, 500);
			base.Body.PlayMovie("beatC", 0, 1000);
			base.Body.CallFuction(method_3, 2200);
		}

		private void method_3()
		{
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				int x = base.Game.Random.Next(200, 1300);
				list_1.Add(new Point(x, 500));
				allLivingPlayer.BoltMove(x, 500, 50);
			}
			CreateFlameEffect();
			base.Body.RangeAttacking(base.Body.X - 10000, base.Body.Y + 10000, "cry", 100, null);
		}

		private void method_4()
		{
			int num = base.Game.Random.Next(0, string_2.Length);
			((SimpleBoss)base.Body).Say(string_2[num], 1, 500);
			base.Body.ChangeDirection(base.Body.FindDirection(simpleNpc_0), 100);
			if (base.Body.ShootPoint(simpleNpc_0.X, simpleNpc_0.Y, 53, 1000, 10000, 1, 2f, 2550))
			{
				base.Body.PlayMovie("aim", 1300, 0);
			}
			base.Body.PlayMovie("beatA", 2000, 0);
			simpleNpc_0.PlayMovie("die", 2000, 0);
			if (simpleNpc_0.X <= 700)
			{
				simpleNpc_0.JumpTo(1225, 563, "born", 5000, 0, 100, null, 1);
				simpleNpc_0.ChangeDirection(-1, 6000);
				base.Body.ChangeDirection(1, 6000);
			}
			else
			{
				simpleNpc_0.JumpTo(468, 555, "born", 5000, 0, 100, null, 1);
				simpleNpc_0.ChangeDirection(1, 6000);
				base.Body.ChangeDirection(-1, 6000);
			}
			((PVEGame)base.Game).SendObjectFocus(simpleNpc_0, 1, 5000, 0);
			num = base.Game.Random.Next(0, string_3.Length);
			simpleNpc_0.Say(string_3[num], 1, 6500, 2000);
		}

		private void method_5()
		{
			base.Body.Say("Thể xác ốm yếu này, đưa ta mượn tạm xem!", 0, 4000);
			base.Body.MoveTo(761, 583, "walk", 5000);
			base.Body.Say("Lửa địa ngục hãy cháy lên!!!", 0, 6000);
			base.Body.PlayMovie("beatC", 6000, 0);
			Player[] allPlayers = base.Game.GetAllPlayers();
			Player[] array = allPlayers;
			foreach (Player player in array)
			{
				if (player?.IsLiving ?? false)
				{
					player.AddEffect(new ContinueReduceBloodEffect(200, int_0, null), 7800);
				}
			}
			base.Body.CallFuction(method_6, 8300);
		}

		private void method_6()
		{
			LivingConfig livingConfig = ((PVEGame)base.Game).BaseLivingConfig();
			livingConfig.CanTakeDamage = false;
			simpleNpc_0 = ((PVEGame)base.Game).CreateNpc(int_2, 468, 555, 1, 1, livingConfig);
			simpleNpc_0.Say("Lửa vĩnh cửu thật khó bị dập tắt. Hãy theo sát tôi, tôi sẽ làm giảm sức mạnh của chúng", 0, 1500, 3000);
			int_4 = 5;
		}

		public void CreateFlameEffect()
		{
			foreach (Point item2 in list_1)
			{
				PhysicalObj item = ((PVEGame)base.Game).Createlayer(item2.X, item2.Y, "", "game.assetmap.Flame", "", 1, 1);
				list_0.Add(item);
			}
			list_1 = new List<Point>();
			base.Body.CallFuction(Remove, 2000);
		}

		public void Remove()
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

		public void KillAttack(int fx, int mx)
		{
			base.Body.CurrentDamagePlus = 1000f;
			int num = base.Game.Random.Next(0, string_4.Length);
			((SimpleBoss)base.Body).Say(string_4[num], 1, 500);
			base.Body.PlayMovie("beatC", 2500, 0);
			base.Body.RangeAttacking(fx, mx, "cry", 3300, null);
		}

		public ThirdTerrorKingFour()
		{
			int_0 = 1000;
			int_1 = 4;
			int_2 = 3318;
			list_0 = new List<PhysicalObj>();
			list_1 = new List<Point>();
			orchins = new List<SimpleNpc>();
		}

		static ThirdTerrorKingFour()
		{
			string_0 = new string[2]
			{
				"Tiếng gầm của hổ ...！",
				"Cảm nhận sự đau đớn của cổ họng！ "
			};
			string_1 = new string[3]
			{
				"Lửa địa ngục...",
				"Tam nhị chân hỏa !",
				"Đốt ngươi chết luôn"
			};
			string_2 = new string[2]
			{
				"Xem đây，<br/>Cây đậu đáng ghét!!",
				"Biến khỏi đây không ??"
			};
			string_3 = new string[3]
			{
				"Thật nguy hiểm!!",
				"Ta không chịu thua ngươi đâu!",
				"Không bao giờ khuất phục."
			};
			string_4 = new string[3]
			{
				"Dám đến gần ta, chết đi...",
				"Nhìn mặt mà ngu vãi!!!",
				"Ta dẫm cho nát bét!!!!"
			};
			string_5 = new string[2]
			{
				"Chạy đường nào đây?",
				"Tìm chỗ mà núp đi nhé!!"
			};
			string_6 = new string[1]
			{
				"Lửa bất diệt cháy bừng lên đi!"
			};
		}
	}
}
