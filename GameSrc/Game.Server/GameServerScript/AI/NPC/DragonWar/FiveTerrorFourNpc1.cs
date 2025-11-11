using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class FiveTerrorFourNpc1 : ABrain
	{
		private int int_0;

		protected Player m_targer;

		private SimpleBoss simpleBoss_0;

		private int int_1;

		private int int_2;

		private static string[] string_0;

		private static string[] string_1;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			m_body.CurrentDamagePlus = 1f;
			m_body.CurrentShootMinus = 1f;
			if (simpleBoss_0 == null)
			{
				SimpleBoss[] array = ((PVEGame)base.Game).FindLivingTurnBossWithID(int_1);
				if (array.Length != 0)
				{
					simpleBoss_0 = array[0];
				}
			}
		}

		public override void OnCreated()
		{
			base.OnCreated();
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			if (simpleBoss_0 != null && (int)simpleBoss_0.Properties1 == 1 && (int)simpleBoss_0.Properties2 == 0)
			{
				if (((PVEGame)base.Game).GetNPCLivingWithID(int_2).Length == 0)
				{
					((SimpleBoss)base.Body).RandomSay(string_0, 0, 1000, 0);
					if (base.Body.ShootPoint(1390, 709, 56, 1000, 10000, 1, 1.5f, 2700, method_2))
					{
						base.Body.PlayMovie("beatA", 1500, 0);
					}
				}
			}
			else if (simpleBoss_0 != null && (int)simpleBoss_0.Properties2 == 1)
			{
				method_0();
			}
			else if (simpleBoss_0 != null && (int)simpleBoss_0.Properties1 > 0 && (int)simpleBoss_0.Properties2 > 0)
			{
				method_0();
			}
		}

		private void method_0()
		{
			((SimpleBoss)base.Body).RandomSay(string_1, 0, 1000, 0);
			base.Body.PlayMovie("beatC", 1500, 4000);
			base.Body.BeatDirect(simpleBoss_0, "", 3500, 3, 1);
			int_0++;
			if (int_0 >= 2)
			{
				int_0 = 0;
				simpleBoss_0.Properties2 = 3;
				simpleBoss_0.BlockTurn = false;
				base.Body.CallFuction(method_1, 5000);
			}
		}

		private void method_1()
		{
			if (simpleBoss_0.IsLiving)
			{
				simpleBoss_0.PlayMovie("dao", 0, 6000);
			}
		}

		private void method_2()
		{
			LivingConfig livingConfig = ((PVEGame)base.Game).BaseLivingConfig();
			livingConfig.IsHelper = true;
			livingConfig.IsTurn = false;
			livingConfig.CanTakeDamage = false;
			((SimpleBoss)base.Body).CreateChild(int_2, 1340, 709, true, livingConfig);
		}

		public override void OnDie()
		{
			base.OnDie();
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		public FiveTerrorFourNpc1()
		{
			int_1 = 5331;
			int_2 = 5334;
		}

		static FiveTerrorFourNpc1()
		{
			string_0 = new string[3]
			{
				"Chú ý tường băng sắp vỡ rồi.",
				"Hãy giúp ta xây tường băng nào.",
				"Xây lại tường băng nhanh lên."
			};
			string_1 = new string[4]
			{
				"Thử cái này xem.",
				"Con rồng ngu hãy coi đây.",
				"Ta bắn chít ngươi.",
				"Sống sao với dàn đạn của ta?"
			};
		}
	}
}
