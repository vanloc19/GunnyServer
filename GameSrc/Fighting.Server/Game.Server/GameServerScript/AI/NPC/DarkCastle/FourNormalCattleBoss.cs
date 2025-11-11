using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class FourNormalCattleBoss : ABrain
	{
		private int lkuwLcxovti;

		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

		private int int_0;

		private int int_1;

		private float float_0;

		private int int_2;

		private bool bool_0;

		private Player player_0;

		private float float_1;

		private float float_2;

		private int int_3;

		private string[] string_0;

		private string[] string_1;

		private string[] string_2;

		private string[] string_3;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			lcjwAebwpHw();
			base.Body.CurrentShootMinus = 1f;
			if (!bool_0)
			{
				base.Body.Config.HaveShield = true;
			}
			else
			{
				base.Body.Config.HaveShield = false;
			}
		}

		public override void OnCreated()
		{
			base.OnCreated();
			bool_0 = false;
			base.Body.CurrentDamagePlus = 1f;
			float_2 = 1f;
		}

		public override void OnStartAttacking()
		{
			if (bool_0 && int_3 > 0)
			{
				int_3--;
				if (int_3 <= 0)
				{
					lkuwLcxovti = 3;
				}
				return;
			}
			if (!bool_0)
			{
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
					method_0(base.Body.X - 200, base.Body.X + 200);
					return;
				}
			}
			if (lkuwLcxovti == 0)
			{
				if (!bool_0)
				{
					base.Body.CallFuction(method_6, 2000);
					if (((SimpleBoss)base.Body).CurrentLivingNpcNum <= 0)
					{
						base.Body.Say("Lửa địa ngục đâu, ra mau.", 0, 5000);
						base.Body.CallFuction(method_8, 7000);
					}
					else
					{
						base.Body.CallFuction(method_2, 4000);
					}
				}
				lkuwLcxovti++;
			}
			else if (lkuwLcxovti == 1)
			{
				if (!bool_0)
				{
					base.Body.CallFuction(method_6, 2000);
					base.Body.CallFuction(method_1, 4000);
					lkuwLcxovti++;
				}
				else
				{
					lkuwLcxovti = 3;
				}
			}
			else if (lkuwLcxovti == 2)
			{
				if (!bool_0)
				{
					base.Body.CallFuction(method_6, 2000);
					base.Body.CallFuction(method_2, 4000);
				}
				lkuwLcxovti++;
			}
			else
			{
				if (lkuwLcxovti != 3)
				{
					return;
				}
				if (!bool_0)
				{
					base.Body.CallFuction(method_6, 2000);
					if (((SimpleBoss)base.Body).CurrentLivingNpcNum <= 0)
					{
						bool_0 = true;
						int_3 = 2;
						base.Body.Say("Hơ hơ... Ta bị ốm à??", 0, 2200);
						base.Body.CallFuction(method_3, 4000);
					}
					else
					{
						base.Body.CallFuction(method_5, 4000);
					}
				}
				else
				{
					bool_0 = false;
					float_2 = 1f;
					base.Body.CallFuction(method_4, 2000);
				}
				lkuwLcxovti = 0;
			}
		}

		private void method_0(int int_4, int int_5)
		{
			float_1 = base.Body.CurrentDamagePlus;
			base.Body.CurrentDamagePlus = 1000f;
			base.Body.ChangeDirection(base.Game.FindlivingbyDir(base.Body), 100);
			((SimpleBoss)base.Body).RandomSay(string_3, 0, 2000, 0);
			base.Body.PlayMovie("beatC", 2000, 0);
			base.Body.PlayMovie("beatE", 5000, 0);
			base.Body.RangeAttacking(int_4, int_5, "cry", 7000, null);
			base.Body.CallFuction(method_10, 8000);
		}

		private void method_1()
		{
			player_0 = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
			if (player_0 != null)
			{
				base.Body.ChangeDirection(player_0, 100);
				base.Body.Say("Tên ranh kia hãy đỡ này!!!", 0, 1000);
				base.Body.PlayMovie("beatA", 1200, 0);
				((PVEGame)base.Game).SendObjectFocus(player_0, 1, 3200, 0);
				base.Body.CallFuction(method_7, 4000);
				if (base.Body.FindDirection(player_0) == -1)
				{
					base.Body.RangeAttacking(player_0.X - 50, base.Body.X, "cry", 4800, null);
				}
				else
				{
					base.Body.RangeAttacking(base.Body.X, player_0.X + 50, "cry", 4800, null);
				}
				base.Body.CallFuction(method_10, 6000);
			}
		}

		private void method_2()
		{
			((SimpleBoss)base.Body).RandomSay(string_0, 0, 1000, 0);
			base.Body.PlayMovie("beatB", 1000, 0);
			base.Body.RangeAttacking(base.Body.X - 10000, base.Body.X + 10000, "cry", 4100, null);
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				allLivingPlayer.AddEffect(new ReduceStrengthEffect(1, int_2), 4200);
			}
			base.Body.CallFuction(method_10, 5000);
		}

		private void method_3()
		{
			base.Body.PlayMovie("beatD", 1000, 0);
			((SimpleBoss)base.Body).RandomSay(string_2, 0, 4100, 0);
			base.Body.PlayMovie("AtoB", 4000, 0);
			base.Body.CallFuction(method_10, 7000);
		}

		private void method_4()
		{
			base.Body.PlayMovie("AtoB", 1000, 0);
			((SimpleBoss)base.Body).RandomSay(string_1, 0, 2200, 0);
			base.Body.CallFuction(method_5, 2000);
		}

		private void method_5()
		{
			player_0 = base.Game.FindRandomPlayer();
			if (player_0 != null)
			{
				base.Body.PlayMovie("jump", 500, 0);
				((PVEGame)base.Game).SendObjectFocus(player_0, 1, 2000, 0);
				base.Body.BoltMove(player_0.X, player_0.Y, 2500);
				base.Body.PlayMovie("fall", 2600, 0);
				base.Body.RangeAttacking(player_0.X - 100, player_0.X + 100, "cry", 3000, null);
				base.Body.CallFuction(method_10, 4000);
			}
		}

		private void method_6()
		{
			float_2 += float_0;
			base.Body.CurrentDamagePlus = float_2;
			physicalObj_1 = ((PVEGame)base.Game).Createlayer(base.Body.X, base.Body.Y - 60, "", "game.crazytank.assetmap.Buff_powup", "", 1, 0);
		}

		private void method_7()
		{
			if (player_0 != null)
			{
				physicalObj_0 = ((PVEGame)base.Game).Createlayer(player_0.X, player_0.Y, "", "asset.game.4.blade", "", 1, 0);
			}
		}

		private void lcjwAebwpHw()
		{
			if (physicalObj_1 != null)
			{
				base.Game.RemovePhysicalObj(physicalObj_1, true);
			}
			if (physicalObj_0 != null)
			{
				base.Game.RemovePhysicalObj(physicalObj_0, true);
			}
		}

		private void method_8()
		{
			LivingConfig livingConfig = ((PVEGame)base.Game).BaseLivingConfig();
			livingConfig.IsFly = true;
			for (int i = 0; i < int_0; i++)
			{
				int x = base.Game.Random.Next(350, 1300);
				int y = base.Game.Random.Next(100, 700);
				((SimpleBoss)base.Body).CreateChild(int_1, x, y, showBlood: true, livingConfig);
			}
		}

		private void method_9()
		{
			((SimpleBoss)base.Body).RemoveAllChild();
		}

		private void method_10()
		{
			if (bool_0)
			{
				((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standB");
			}
			else
			{
				((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standA");
			}
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		public FourNormalCattleBoss()
		{
			int_0 = 3;
			int_1 = 4107;
			float_0 = 0.5f;
			int_2 = 50;
			string_0 = new string[4]
			{
				"Các ngươi đỡ được không?",
				"Chết chắc rồi bọn ngu si!",
				"Sao mà đỡ lại được đây?",
				"Sao? Sao? Chết đi!!!!!"
			};
			string_1 = new string[4]
			{
				"Xem ngươi chạy đâu cho thoát",
				"Dám đánh nén ta à?",
				"Để xem các ngươi chạy đâu",
				"Đánh nén ta là điều không thể tha thứ"
			};
			string_2 = new string[3]
			{
				"Mệt rồi!",
				"Ta cảm thấy mệt quá!",
				"Mệt quá! Nghỉ tí đã."
			};
			string_3 = new string[4]
			{
				"Để ta tiễn ngươi!",
				"Chết chắc rồi cưng à!",
				"Ngươi không chết mới là lạ.",
				"Ta sẽ ban tặng ngươi cái chết êm ái"
			};
		}
	}
}
