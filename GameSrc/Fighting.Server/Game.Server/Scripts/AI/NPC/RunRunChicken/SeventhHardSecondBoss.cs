using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class SeventhHardSecondBoss : ABrain
	{
		private int int_0;

		protected Player m_targer;

		private PhysicalObj PqowMlrYojm;

		private PhysicalObj physicalObj_0;

		private int int_1;

		private static string[] HumwMzwXiki;

		private static string[] string_0;

		private static string[] string_1;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			m_body.CurrentDamagePlus = 3f;
			m_body.CurrentShootMinus = 3f;
		}

		public override void OnCreated()
		{
			base.OnCreated();
			base.Body.MaxBeatDis = 300;
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
			else if (int_0 == 0)
			{
				method_1();
				int_0++;
			}
			else if (int_0 == 1)
			{
				method_2();
				int_0++;
			}
			else if (int_0 == 2)
			{
				method_4();
				int_0++;
			}
			else
			{
				method_3();
				int_0 = 0;
			}
		}

		private void method_0(int int_2, int int_3)
		{
			int num = base.Game.Random.Next(0, string_1.Length);
			base.Body.CurrentDamagePlus = 1000f;
			base.Body.Say(string_1[num], 1, 0);
			base.Body.PlayMovie("speak", 0, 0);
			base.Body.RangeAttacking(int_2, int_3, "cry", 2000, null);
		}

		private void method_1()
		{
			m_targer = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
			if (m_targer != null)
			{
				base.Body.ChangeDirection(m_targer, 100);
				base.Body.MoveTo(m_targer.X - 100, m_targer.Y, "run", 1000, method_5, 10);
			}
		}

		private void method_2()
		{
			m_targer = base.Game.FindRandomPlayer();
			base.Body.CurrentDamagePlus = 3f;
			if (m_targer != null)
			{
				base.Body.ChangeDirection(m_targer, 100);
				base.Body.MoveTo(m_targer.X - 200, m_targer.Y, "run", 1000, method_6, 10);
			}
		}

		private void method_3()
		{
			m_targer = base.Game.FindRandomPlayer();
			base.Body.CurrentDamagePlus = 1.5f;
			if (m_targer != null)
			{
				((SimpleBoss)base.Body).RandomSay(string_0, 0, 1000, 0);
				if (base.Body.ShootPoint(m_targer.X, m_targer.Y, 83, 1000, 10000, 2, 2f, 3000))
				{
					base.Body.PlayMovie("beatC", 1300, 0);
				}
			}
		}

		private void method_4()
		{
			base.Body.MoveTo(1484, 950, "run", 1000, method_7, 10);
		}

		private void method_5()
		{
			if (m_targer != null)
			{
				base.Body.Beat(m_targer, "beatB", 100, 0, 100);
				base.Body.CallFuction(method_12, 3000);
			}
		}

		private void method_6()
		{
			if (m_targer != null)
			{
				((SimpleBoss)base.Body).RandomSay(HumwMzwXiki, 0, 50, 0);
				base.Body.Beat(m_targer, "beatA", 100, 0, 100);
				base.Body.CallFuction(method_12, 3000);
			}
		}

		private void method_7()
		{
			base.Body.ChangeDirection(-1, 0);
			base.Body.PlayMovie("beatD", 500, 0);
			base.Body.Say("Hãy nếm thử món ăn đặc biệt này.", 4500, 0);
			base.Body.CallFuction(method_8, 5000);
		}

		private void method_8()
		{
			method_10();
			m_targer = base.Game.FindRandomPlayer();
			if (m_targer != null)
			{
				((PVEGame)base.Game).SendObjectFocus(m_targer, 0, 0, 0);
				base.Body.CallFuction(method_9, 1000);
				foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
				{
					if (allLivingPlayer.X > m_targer.X - 180 && allLivingPlayer.X < m_targer.X + 180)
					{
						allLivingPlayer.AddEffect(new ContinueReduceGreenBloodEffect(5, int_1, base.Body), 1500);
					}
				}
				base.Body.CallFuction(method_12, 3000);
			}
		}

		private void method_9()
		{
			if (m_targer != null)
			{
				PqowMlrYojm = ((PVEGame)base.Game).Createlayer(m_targer.X, m_targer.Y, "", "asset.game.seven.choud", "", 1, 1);
				physicalObj_0 = ((PVEGame)base.Game).Createlayer(m_targer.X, m_targer.Y, "", "asset.game.seven.du", "", 1, 1);
				base.Body.CallFuction(method_11, 3000);
			}
		}

		private void method_10()
		{
			if (physicalObj_0 != null)
			{
				base.Game.RemovePhysicalObj(physicalObj_0, sendToClient: true);
			}
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				(allLivingPlayer.EffectList.GetOfType(eEffectType.ContinueReduceGreenBloodEffect) as ContinueReduceGreenBloodEffect)?.Stop();
			}
		}

		private void method_11()
		{
			if (PqowMlrYojm != null)
			{
				base.Game.RemovePhysicalObj(PqowMlrYojm, sendToClient: true);
			}
		}

		private void method_12()
		{
			base.Body.MoveTo(200, 590, "run", 100, method_13, 11);
		}

		private void method_13()
		{
			if (base.Body.X > 1000)
			{
				base.Body.ChangeDirection(-1, 100);
			}
			else
			{
				base.Body.ChangeDirection(1, 100);
			}
		}

		public override void OnDie()
		{
			base.OnDie();
			method_10();
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		public SeventhHardSecondBoss()
		{

			int_1 = 1000;

		}

		static SeventhHardSecondBoss()
		{

			HumwMzwXiki = new string[3]
			{
				"Aaaaa!!! Trời sập.",
				"Mông bự mông to!!",
				"Đè chết ngươi nè"
			};
			string_0 = new string[3]
			{
				"Ăn trước luộc hơm?",
				"Trứng bay!! Trứng rơi!!!",
				"Đỡ những quả trứng bá đạo của ta!"
			};
			string_1 = new string[2]
			{
				"Đến nộp mạng à ?? Sức mạnh tối cao!!...",
				"Gần ta là chết chắc rồi."
			};
		}
	}
}
