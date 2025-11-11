using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class DTGT1165 : AMissionControl
	{
		private SimpleBoss simpleBoss_0;

		private int sjhwejPgAuU;

		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

		private PhysicalObj physicalObj_2;

		private PhysicalObj dwhweqOgfDi;

		private PhysicalObj physicalObj_3;

		private PhysicalObj physicalObj_4;

		private PhysicalObj physicalObj_5;

		private PhysicalObj physicalObj_6;

		private PhysicalObj NfhwelRpBmt;

		private PhysicalObj physicalObj_7;

		private PhysicalObj physicalObj_8;

		private PhysicalObj physicalObj_9;

		private PhysicalObj physicalObj_10;

		private PhysicalObj physicalObj_11;

		private PhysicalObj physicalObj_12;

		private PhysicalObj physicalObj_13;

		private PhysicalObj physicalObj_14;

		private PhysicalObj physicalObj_15;

		private PhysicalObj physicalObj_16;

		private PhysicalObj physicalObj_17;

		private int int_0;

		private int[] int_1;

		private int[] int_2;

		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			if (score > 900)
			{
				return 3;
			}
			if (score > 825)
			{
				return 2;
			}
			if (score > 725)
			{
				return 1;
			}
			return 0;
		}

		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			int[] npcIds = new int[1]
			{
				sjhwejPgAuU
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds);
			base.Game.AddLoadingFile(2, "image/game/thing/bossborn6.swf", "game.asset.living.GuizeAsset");
			base.Game.AddLoadingFile(2, "image/game/effect/6/ball.swf", "asset.game.six.ball");
			base.Game.AddLoadingFile(2, "image/game/effect/6/jifenpai.swf", "asset.game.six.fenshu");
			base.Game.AddLoadingFile(2, "image/game/effect/6/jifenpai.swf", "asset.game.six.shuzi");
			base.Game.SetMap(1165);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			physicalObj_2 = base.Game.Createlayer(110, 669, "hide", "asset.game.six.fenshu", "Z", 1, 1);
			physicalObj_2.CanPenetrate = true;
			dwhweqOgfDi = base.Game.Createlayer(1500, 669, "hide", "asset.game.six.fenshu", "Z", 1, 1);
			dwhweqOgfDi.CanPenetrate = true;
			physicalObj_3 = base.Game.Createlayer(185, 669, "hide", "asset.game.six.shuzi", "z0", 1, 1);
			physicalObj_3.CanPenetrate = true;
			physicalObj_4 = base.Game.Createlayer(240, 669, "hide", "asset.game.six.shuzi", "z0", 1, 1);
			physicalObj_4.CanPenetrate = true;
			physicalObj_5 = base.Game.Createlayer(1575, 669, "hide", "asset.game.six.shuzi", "z0", 1, 1);
			physicalObj_5.CanPenetrate = true;
			physicalObj_6 = base.Game.Createlayer(1630, 669, "hide", "asset.game.six.shuzi", "z0", 1, 1);
			physicalObj_6.CanPenetrate = true;
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsTurn = false;
			livingConfig.CanTakeDamage = false;
			simpleBoss_0 = base.Game.CreateBoss(sjhwejPgAuU, 345, 860, -1, 10, "", livingConfig);
			simpleBoss_0.PlayMovie("standC", 0, 0);
			simpleBoss_0.PlayMovie("go", 1000, 0);
			simpleBoss_0.Say("Ô ! Ngưới mới à, chắc bạn không biết quy tắc ở đây.", 0, 0);
			simpleBoss_0.CallFuction(method_0, 4000);
			simpleBoss_0.CallFuction(method_1, 5000);
		}

		private void method_0()
		{
			base.Game.SendGameFocus(900, 500, 1, 0, 1000);
			simpleBoss_0.CallFuction(method_2, 6000);
		}

		private void method_1()
		{
			physicalObj_1 = base.Game.Createlayer(900, 450, "hide", "game.asset.living.GuizeAsset", "out", 1, 1);
			physicalObj_1.CanPenetrate = true;
		}

		private void method_2()
		{
			int num = base.Game.Random.Next(-2, 6);
			int num2 = base.Game.Random.Next(-2, 6);
			int num3 = base.Game.Random.Next(-2, 6);
			int num4 = base.Game.Random.Next(-3, 6);
			int num5 = base.Game.Random.Next(-1, 6);
			int num6 = base.Game.Random.Next(-1, 6);
			int num7 = base.Game.Random.Next(-1, 6);
			int num8 = base.Game.Random.Next(-4, 6);
			int num9 = base.Game.Random.Next(1, 6);
			int num10 = base.Game.Random.Next(1, 6);
			int num11 = base.Game.Random.Next(1, 6);
			int num12 = base.Game.Random.Next(-6, 6);
			NfhwelRpBmt = base.Game.CreateBall(850, 300, "shield" + num, "s-" + num, 1, 0);
			physicalObj_7 = base.Game.CreateBall(750, 400, "shield" + num2, "s-" + num2, 1, 0);
			physicalObj_8 = base.Game.CreateBall(650, 300, "shield" + num3, "s-" + num3, 1, 0);
			physicalObj_9 = base.Game.CreateBall(950, 400, "shield" + num4, "s-" + num4, 1, 0);
			physicalObj_10 = base.Game.CreateBall(1050, 300, "shield" + num5, "s-" + num5, 1, 0);
			physicalObj_11 = base.Game.CreateBall(1150, 400, "shield" + num6, "s-" + num6, 1, 0);
			physicalObj_12 = base.Game.CreateBall(850, 500, "shield" + num7, "s" + num7, 1, 0);
			physicalObj_13 = base.Game.CreateBall(750, 600, "shield" + num8, "s" + num8, 1, 0);
			physicalObj_14 = base.Game.CreateBall(650, 500, "shield" + num9, "s" + num9, 1, 0);
			physicalObj_15 = base.Game.CreateBall(950, 600, "shield" + num10, "s" + num10, 1, 0);
			physicalObj_16 = base.Game.CreateBall(1050, 500, "shield" + num11, "s" + num11, 1, 0);
			physicalObj_17 = base.Game.CreateBall(1150, 600, "shield" + num12, "s" + num12, 1, 0);
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
			base.Game.RemovePhysicalObj(NfhwelRpBmt, true);
			base.Game.RemovePhysicalObj(physicalObj_7, true);
			base.Game.RemovePhysicalObj(physicalObj_8, true);
			base.Game.RemovePhysicalObj(physicalObj_9, true);
			base.Game.RemovePhysicalObj(physicalObj_10, true);
			base.Game.RemovePhysicalObj(physicalObj_11, true);
			base.Game.RemovePhysicalObj(physicalObj_12, true);
			base.Game.RemovePhysicalObj(physicalObj_13, true);
			base.Game.RemovePhysicalObj(physicalObj_14, true);
			base.Game.RemovePhysicalObj(physicalObj_15, true);
			base.Game.RemovePhysicalObj(physicalObj_16, true);
			base.Game.RemovePhysicalObj(physicalObj_17, true);
			simpleBoss_0.CallFuction(method_2, 500);
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			if (base.Game.TurnIndex > int_0 + 1 && physicalObj_1 != null)
			{
				physicalObj_1.Die();
				base.Game.RemovePhysicalObj(physicalObj_1, true);
				physicalObj_1 = null;
			}
			if (base.Game.TotalKillCount > 99)
			{
				base.Game.TotalKillCount = 99;
			}
			string text = base.Game.TotalKillCount.ToString();
			if (text.Length == 1)
			{
				text = "0" + text;
			}
			physicalObj_3.PlayMovie("z" + text[0].ToString(), 100, 500);
			physicalObj_4.PlayMovie("z" + text[1].ToString(), 100, 500);
			physicalObj_5.PlayMovie("z" + text[0].ToString(), 100, 500);
			physicalObj_6.PlayMovie("z" + text[1].ToString(), 100, 500);
		}

		public override bool CanGameOver()
		{
			base.CanGameOver();
			if (base.Game.TotalKillCount >= 99)
			{
				base.Game.TotalKillCount = 99;
				return true;
			}
			if (base.Game.TotalTurn > 200)
			{
				return true;
			}
			return false;
		}

		public override int UpdateUIData()
		{
			return base.Game.TotalKillCount;
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (base.Game.TotalKillCount >= 99)
			{
				base.Game.IsWin = true;
			}
			else if (base.Game.TotalTurn > 200)
			{
				base.Game.IsWin = false;
			}
		}

		public DTGT1165()
		{

			sjhwejPgAuU = 6121;
			int_1 = new int[18]
			{
				450,
				550,
				650,
				750,
				850,
				950,
				1050,
				1150,
				1250,
				455,
				555,
				655,
				755,
				855,
				955,
				1055,
				1155,
				1255
			};
			int_2 = new int[5]
			{
				184,
				259,
				335,
				420,
				504
			};

		}
	}
}
