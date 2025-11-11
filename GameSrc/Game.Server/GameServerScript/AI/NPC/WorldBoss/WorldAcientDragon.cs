using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GameServerScript.AI.NPC
{
	public class WorldAcientDragon : ABrain
	{
		private int int_0;

		private Point[] uKqsylCjiHL;

		private static string[] string_0;

		private static string[] string_1;

		private static string[] string_2;

		private static string[] string_3;

		private static string[] string_4;

		private static string[] qEqsweReoZa;

		private static string[] Qufswhqpad0;

		private static string[] string_5;

		static WorldAcientDragon()
		{

			WorldAcientDragon.string_0 = new string[] { LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg1", Array.Empty<object>()), LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg2", Array.Empty<object>()), LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg3", Array.Empty<object>()) };
			WorldAcientDragon.string_1 = new string[] { LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg4", Array.Empty<object>()), LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg5", Array.Empty<object>()) };
			WorldAcientDragon.string_2 = new string[] { LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg6", Array.Empty<object>()), LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg7", Array.Empty<object>()) };
			WorldAcientDragon.string_3 = new string[] { LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg8", Array.Empty<object>()), LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg9", Array.Empty<object>()) };
			WorldAcientDragon.string_4 = new string[] { LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg10", Array.Empty<object>()), LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg11", Array.Empty<object>()), LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg12", Array.Empty<object>()) };
			WorldAcientDragon.qEqsweReoZa = new string[] { LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg13", Array.Empty<object>()), LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg14", Array.Empty<object>()) };
			WorldAcientDragon.Qufswhqpad0 = new string[] { LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg15", Array.Empty<object>()), LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg16", Array.Empty<object>()) };
			WorldAcientDragon.string_5 = new string[] { LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg17", Array.Empty<object>()) };
		}

		public WorldAcientDragon()
		{

			this.uKqsylCjiHL = new Point[] { new Point(979, 630), new Point(1013, 630), new Point(1052, 630), new Point(1088, 630), new Point(1142, 630) };

		}

		public override void OnBeforeTakedDamage(Living source, ref int damageAmount, ref int criticalAmount)
		{
			base.OnBeforeTakedDamage(source, ref damageAmount, ref criticalAmount);
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				//allFightPlayer.PlayerDetail.UpdateWorldBossBlood(damageAmount + criticalAmount);
			}
		}

		private void method_0(int int_1, int int_2)
		{
			int num = base.Game.Random.Next(0, (int)WorldAcientDragon.qEqsweReoZa.Length);
			base.Body.Say(WorldAcientDragon.qEqsweReoZa[num], 1, 1000);
			base.Body.CurrentDamagePlus = 100f;
			base.Body.PlayMovie("beatF", 3000, 0);
			base.Body.RangeAttacking(int_1, int_2, "cry", 5000, null);
		}

		private void method_1()
		{
			if (base.Game.FindRandomPlayer() != null)
			{
				int num = base.Game.Random.Next(0, (int)WorldAcientDragon.qEqsweReoZa.Length);
				base.Body.Say(WorldAcientDragon.qEqsweReoZa[num], 1, 1000);
				base.Body.CurrentDamagePlus = 15f;
				base.Body.PlayMovie("beatF", 3000, 0);
				base.Body.RangeAttacking(0, base.Body.X + 1000, "cry", 5000, null);
			}
		}

		private void method_2()
		{
			if (base.Game.FindRandomPlayer() != null)
			{
				base.Body.CurrentDamagePlus = 5f;
				int num = base.Game.Random.Next(0, (int)WorldAcientDragon.string_1.Length);
				base.Body.Say(WorldAcientDragon.string_1[num], 1, 0);
				base.Game.Random.Next(0, 1200);
				base.Body.PlayMovie("beatC", 1700, 0);
				base.Body.RangeAttacking(0, base.Body.X + 1000, "cry", 4000, null);
			}
		}

		private void method_3()
		{
			if (base.Game.FindRandomPlayer() != null)
			{
				base.Body.CurrentDamagePlus = 10f;
				int num = base.Game.Random.Next(0, (int)WorldAcientDragon.string_1.Length);
				base.Body.Say(WorldAcientDragon.string_1[num], 1, 0);
				base.Game.Random.Next(0, 1200);
				base.Body.PlayMovie("beatE", 1700, 0);
				base.Body.RangeAttacking(0, base.Body.X + 1000, "cry", 4000, null);
			}
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			base.Body.CurrentDamagePlus = 1f;
			base.Body.CurrentShootMinus = 1f;
		}

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
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
				if (!allFightPlayer.IsLiving || allFightPlayer.X <= 1000)
				{
					continue;
				}
				int num1 = (int)base.Body.Distance(allFightPlayer.X, allFightPlayer.Y);
				if (num1 > num)
				{
					num = num1;
				}
				flag = true;
			}
			if (flag)
			{
				this.method_0(0, base.Game.Map.Info.ForegroundWidth + 1);
				return;
			}
			if (this.int_0 == 0)
			{
				this.method_2();
				this.int_0++;
				return;
			}
			if (this.int_0 != 1)
			{
				this.method_1();
				this.int_0 = 0;
				return;
			}
			this.method_3();
			this.int_0++;
		}
	}
}