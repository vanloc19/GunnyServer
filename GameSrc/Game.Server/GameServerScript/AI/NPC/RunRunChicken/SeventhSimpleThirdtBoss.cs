using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class SeventhSimpleThirdtBoss : ABrain
	{
		private int int_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private PhysicalObj physicalObj_0;

		private int int_4;

		private static string[] string_0;

		private static string[] string_1;

		private static string[] string_2;

		private static string[] string_3;

		private static string[] string_4;

		private static string[] string_5;

		private static string[] string_6;

		private static string[] string_7;

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
				method_0(1269, base.Game.Map.Info.ForegroundWidth + 1);
			}
			else if (int_0 == 0)
			{
				Angger();
				int_0++;
			}
			else if (int_0 == 1)
			{
				Summon();
				base.Body.State = 0;
				int_0++;
			}
			else if (int_0 == 2)
			{
				Angger();
				int_0++;
			}
			else if (int_0 == 3)
			{
				Angger2();
				int_0++;
			}
			else if (int_0 == 4)
			{
				Summon2();
				base.Body.State = 0;
				int_0++;
			}
			else if (int_0 == 5)
			{
				Angger();
				int_0++;
			}
			else if (int_0 == 6)
			{
				Angger2();
				int_0++;
			}
			else if (int_0 == 7)
			{
				Summon3();
				base.Body.State = 0;
				int_0++;
			}
			else
			{
				Angger();
				int_0 = 0;
			}
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		private void method_0(int int_5, int int_6)
		{
			base.Body.CurrentDamagePlus = 1f;
			base.Body.PlayMovie("beatB", 2000, 0);
			base.Body.RangeAttacking(int_5, int_6, "cry", 3000, null);
		}

		public void Summon()
		{
			base.Body.PlayMovie("Ato", 100, 0);
			base.Body.CallFuction(method_3, 2500);
		}

		public void Summon2()
		{
			base.Body.PlayMovie("Ato", 100, 0);
			base.Body.CallFuction(method_1, 2500);
		}

		public void Summon3()
		{
			base.Body.PlayMovie("Ato", 100, 0);
			base.Body.CallFuction(method_4, 2500);
		}

		private void method_1()
		{
			base.Body.PlayMovie("beatB", 3000, 4000);
			((SimpleBoss)base.Body).SetRelateDemagemRect(-21, -87, 72, 59);
			base.Body.CallFuction(method_2, 7000);
		}

		private void method_2()
		{
			Player player = base.Game.FindRandomPlayer();
			physicalObj_0 = ((PVEGame)base.Game).Createlayer(player.X, player.Y, "moive", "asset.game.seven.cao", "in", 1, 0);
			Player player2 = base.Game.FindRandomPlayer();
			physicalObj_0 = ((PVEGame)base.Game).Createlayer(player2.X, player2.Y, "moive", "asset.game.seven.cao", "in", 1, 0);
			Player player3 = base.Game.FindRandomPlayer();
			physicalObj_0 = ((PVEGame)base.Game).Createlayer(player3.X, player3.Y, "moive", "asset.game.seven.cao", "in", 1, 0);
			Player player4 = base.Game.FindRandomPlayer();
			physicalObj_0 = ((PVEGame)base.Game).Createlayer(player4.X, player4.Y, "moive", "asset.game.seven.cao", "in", 1, 0);
			base.Body.SyncAtTime = true;
			base.Body.PlayMovie("", 1000, 1500);
			int num = base.Game.Random.Next(225, 774);
			player.AddBlood(-num, 1);
		}

		private void method_3()
		{
			Player player = base.Game.FindRandomPlayer();
			((SimpleBoss)base.Body).SetRelateDemagemRect(-21, -87, 72, 59);
			if (player != null)
			{
				base.Body.CurrentDamagePlus = 1.8f;
				base.Game.Random.Next(player.X, player.X);
				if (base.Body.ShootPoint(player.X, player.Y, 84, 1200, 10000, 1, 3f, 2550))
				{
					base.Body.PlayMovie("beatA", 1700, 0);
				}
			}
		}

		public void Angger()
		{
			base.Body.State = 1;
			int_4 += 100;
			base.Body.PlayMovie("toA", 1700, 0);
			((SimpleBoss)base.Body).SetDander(int_4);
			if (base.Body.Direction == -1)
			{
				((SimpleBoss)base.Body).SetRelateDemagemRect(8, -252, 74, 50);
			}
			else
			{
				((SimpleBoss)base.Body).SetRelateDemagemRect(-82, -252, 74, 50);
			}
		}

		public void Angger2()
		{
			base.Body.State = 1;
			int_4 += 100;
			base.Body.PlayMovie("standA", 1700, 0);
			((SimpleBoss)base.Body).SetDander(int_4);
			if (base.Body.Direction == -1)
			{
				((SimpleBoss)base.Body).SetRelateDemagemRect(8, -252, 74, 50);
			}
			else
			{
				((SimpleBoss)base.Body).SetRelateDemagemRect(-82, -252, 74, 50);
			}
		}

		private void method_4()
		{
			Player player = base.Game.FindRandomPlayer();
			((SimpleBoss)base.Body).SetRelateDemagemRect(-21, -87, 72, 59);
			if (player != null)
			{
				base.Body.CurrentDamagePlus = 1.8f;
				base.Game.Random.Next(player.X, player.X);
				if (base.Body.ShootPoint(player.X, player.Y, 84, 1200, 10000, 1, 3f, 3000))
				{
					base.Body.PlayMovie("beat", 1700, 0);
				}
			}
		}

		public SeventhSimpleThirdtBoss()
		{

			int_2 = 1;

		}

		static SeventhSimpleThirdtBoss()
		{

			string_0 = new string[1]
			{
				LanguageMgr.GetTranslation("Ddtank super là số 1")
			};
			string_1 = new string[1]
			{
				LanguageMgr.GetTranslation("Anh em tiến lên !")
			};
			string_2 = new string[1]
			{
				LanguageMgr.GetTranslation("Anh em tiến lên !")
			};
			string_3 = new string[1]
			{
				LanguageMgr.GetTranslation("Ai giết được chúng sẻ được ban thưởng !")
			};
			string_4 = new string[1]
			{
				LanguageMgr.GetTranslation("Ai giết được chúng sẻ được ban thưởng !")
			};
			string_5 = new string[2]
			{
				LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg13"),
				LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg14")
			};
			string_6 = new string[2]
			{
				LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg15"),
				LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg16")
			};
			string_7 = new string[1]
			{
				LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg17")
			};
		}
	}
}
