using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class ThirdTerrorLongNpcFirst : ABrain
	{
		private int int_0;

		private int int_1;

		protected Player m_targer;

		private static string[] GrawYqScVhQ;

		private static string[] string_0;

		private static string[] string_1;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			base.Body.CurrentDamagePlus = 1f;
			base.Body.CurrentShootMinus = 1f;
			int_1 = 0;
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
				if (allFightPlayer.IsLiving && allFightPlayer.X > 1269 && allFightPlayer.X < base.Game.Map.Info.ForegroundWidth + 1)
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
				method_0();
			}
			else if (int_0 == 0)
			{
				if (((PVEGame)base.Game).GetLivedLivings().Count == 9)
				{
					method_3();
				}
				else
				{
					method_3();
				}
				int_0++;
			}
			else
			{
				method_3();
				int_0 = 0;
			}
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		private void method_0()
		{
			m_targer = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
			method_1(m_targer);
		}

		private void method_1(Player player_0)
		{
			int num = (int)player_0.Distance(base.Body.X, base.Body.Y);
			if (player_0.X > base.Body.X)
			{
				base.Body.MoveTo(base.Body.X + num + 50, base.Body.Y, "walk", 1200, "", 4, method_2);
			}
			else
			{
				base.Body.MoveTo(base.Body.X - num - 50, base.Body.Y, "walk", 1200, "", 4, method_2);
			}
		}

		private void method_2()
		{
			base.Body.Beat(m_targer, "beatB", 100, 0, 500, 1, 1);
		}

		private void method_3()
		{
			int x = base.Game.Random.Next(1200, 1550);
			base.Body.MoveTo(x, base.Body.Y, "walk", 1000, "", 4, method_4);
		}

		private void method_4()
		{
			Player player = base.Game.FindRandomPlayer();
			base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
			if (player != null)
			{
				base.Body.CurrentDamagePlus = 1.8f;
				if (base.Body.ShootPoint(player.X, player.Y, 58, 1000, 10000, 1, 2.5f, 2550))
				{
					base.Body.PlayMovie("beatA", 1700, 0);
				}
			}
		}

		public override void OnKillPlayerSay()
		{
			base.OnKillPlayerSay();
			int num = base.Game.Random.Next(0, GrawYqScVhQ.Length);
			base.Body.Say(GrawYqScVhQ[num], 1, 0, 2000);
		}

		public override void OnShootedSay()
		{
			int num = base.Game.Random.Next(0, string_0.Length);
			if (int_1 == 0 && base.Body.IsLiving)
			{
				base.Body.Say(string_0[num], 1, 900, 0);
				int_1 = 1;
			}
			if (!base.Body.IsLiving)
			{
				num = base.Game.Random.Next(0, string_1.Length);
				base.Body.Say(string_1[num], 1, 100, 2000);
			}
		}

		static ThirdTerrorLongNpcFirst()
		{
			GrawYqScVhQ = new string[1]
			{
				LanguageMgr.GetTranslation("Anh em tiến lên !")
			};
			string_0 = new string[2]
			{
				LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg15"),
				LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg16")
			};
			string_1 = new string[1]
			{
				LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg17")
			};
		}
	}
}
