using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class ThirdNormalLongNpcSecond : ABrain
	{
		private int int_0;

		private int int_1;

		private static string[] string_0;

		private static string[] string_1;

		private static string[] string_2;

		private static string[] string_3;

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
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				if (allFightPlayer.IsLiving && allFightPlayer.X > base.Body.X - 300 && allFightPlayer.X < base.Body.X + 300)
				{
					flag = true;
				}
			}
			if (flag)
			{
				method_0(base.Body.X - 300, base.Body.X + 300);
			}
			else if (int_0 == 0)
			{
				if (((PVEGame)base.Game).GetLivedLivings().Count == 9)
				{
					method_1();
				}
				else
				{
					method_1();
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

		private void method_0(int int_2, int int_3)
		{
			int num = base.Game.Random.Next(0, string_1.Length);
			base.Body.Say(string_1[num], 1, 1000);
			base.Body.CurrentDamagePlus = 100f;
			base.Body.PlayMovie("beatB", 3000, 0);
			base.Body.RangeAttacking(int_2, int_3, "cry", 5000, null);
		}

		private void method_1()
		{
			int x = base.Game.Random.Next(50, 200);
			base.Body.MoveTo(x, base.Body.Y, "walk", 1000, "", 4, method_2);
		}

		private void method_2()
		{
			Player player = base.Game.FindRandomPlayer();
			if (player != null)
			{
				base.Body.CurrentDamagePlus = 1.8f;
				if (player.X > base.Body.Y)
				{
					base.Body.ChangeDirection(1, 50);
				}
				else
				{
					base.Body.ChangeDirection(-1, 50);
				}
				base.Game.Random.Next(player.X, player.X);
				if (base.Body.ShootPoint(1100, player.Y, 58, 1000, 10000, 3, 3f, 2550))
				{
					base.Body.PlayMovie("beatA", 1700, 0);
				}
			}
		}

		public override void OnKillPlayerSay()
		{
			base.OnKillPlayerSay();
			int num = base.Game.Random.Next(0, string_0.Length);
			base.Body.Say(string_0[num], 1, 0, 2000);
		}

		public override void OnShootedSay()
		{
			int num = base.Game.Random.Next(0, string_2.Length);
			if (int_1 == 0 && base.Body.IsLiving)
			{
				base.Body.Say(string_2[num], 1, 900, 0);
				int_1 = 1;
			}
			if (!base.Body.IsLiving)
			{
				num = base.Game.Random.Next(0, string_3.Length);
				base.Body.Say(string_3[num], 1, 100, 2000);
			}
		}

		static ThirdNormalLongNpcSecond()
		{
			string_0 = new string[1]
			{
				LanguageMgr.GetTranslation("Anh em tiến lên !")
			};
			string_1 = new string[2]
			{
				LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg13"),
				LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg14")
			};
			string_2 = new string[2]
			{
				LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg15"),
				LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg16")
			};
			string_3 = new string[1]
			{
				LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg17")
			};
		}
	}
}
