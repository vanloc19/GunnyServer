using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;
using System.Drawing;

namespace GameServerScript.AI.Messions
{
	public class DTGT1166 : AMissionControl
	{
		private SimpleBoss simpleBoss_0;

		private SimpleBoss simpleBoss_1;

		private PhysicalObj physicalObj_0;

		private int int_0;

		private int ByuwzhVvRsi;

		private int int_1;

		private int int_2;

		private int int_3;

		private int int_4;

		private SimpleNpc simpleNpc_0;

		private int int_5;

		private int int_6;

		private List<Point> list_0;

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
			base.Game.AddLoadingFile(2, "image/game/effect/6/danti.swf", "asset.game.six.danti");
			base.Game.AddLoadingFile(2, "image/game/effect/6/qunti.swf", "asset.game.six.qunti");
			base.Game.AddLoadingFile(2, "image/game/effect/6/zhaozi.swf", "asset.game.six.zhaozi");
			base.Game.AddLoadingFile(2, "image/game/effect/6/danjia.swf", "asset.game.six.danjia");
			base.Game.AddLoadingFile(2, "image/game/effect/6/qunjia.swf", "asset.game.six.qunjia");
			int[] npcIds = new int[5]
			{
				int_0,
				int_1,
				ByuwzhVvRsi,
				int_2,
				int_3
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds);
			base.Game.SetMap(1166);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			simpleBoss_0 = base.Game.CreateBoss(int_0, 1910, 1080, -1, 1, "");
			simpleBoss_0.Delay = 1;
			simpleBoss_0.Config.CanTakeDamage = false;
			simpleBoss_1 = base.Game.CreateBoss(int_3, 460, 1080, -1, 1, "");
			simpleBoss_1.Config.CanTakeDamage = false;
			simpleBoss_1.Config.IsTurn = false;
			simpleBoss_1.MoveTo(450, 1080, "walk", 500);
			simpleBoss_1.PlayMovie("go", 1500, 0);
			simpleBoss_1.Say("Tấn công đội xanh, bảo vệ đội đỏ.", 0, 2000);
			physicalObj_0 = base.Game.Createlayerboss(1100, 1080, "font", "game.living.Living190", "stand", 1, 0);
			simpleBoss_0.CallFuction(method_0, 2500);
			base.Game.PveGameDelay = 0;
		}

		private void method_0()
		{
			int_4 = 10;
			simpleNpc_0 = base.Game.CreateNpc(ByuwzhVvRsi, 300, 1080, 1, 1);
			simpleNpc_0.Config.IsFly = true;
			simpleNpc_0.Config.MaxStepMove = 6;
			simpleNpc_0.Config.MinBlood = 1;
			simpleNpc_0.Config.FirstStepMove = int_4;
			simpleNpc_0.MoveTo(list_0[0].X, list_0[0].Y, "walk", 0, 8);
			int_4 -= 2;
			simpleNpc_0 = base.Game.CreateNpc(ByuwzhVvRsi, 250, 1080, 1, 1);
			simpleNpc_0.Config.IsFly = true;
			simpleNpc_0.Config.MaxStepMove = 6;
			simpleNpc_0.Config.MinBlood = 1;
			simpleNpc_0.Config.FirstStepMove = int_4;
			simpleNpc_0.MoveTo(list_0[0].X, list_0[0].Y, "walk", 0, 8);
			int_4 -= 2;
			simpleNpc_0 = base.Game.CreateNpc(ByuwzhVvRsi, 200, 1080, 1, 1);
			simpleNpc_0.Config.IsFly = true;
			simpleNpc_0.Config.MaxStepMove = 6;
			simpleNpc_0.Config.MinBlood = 1;
			simpleNpc_0.Config.FirstStepMove = int_4;
			simpleNpc_0.MoveTo(list_0[0].X, list_0[0].Y, "walk", 0, 8);
			int_4 -= 2;
			simpleNpc_0 = base.Game.CreateNpc(ByuwzhVvRsi, 150, 1080, 1, 1);
			simpleNpc_0.Config.IsFly = true;
			simpleNpc_0.Config.MaxStepMove = 6;
			simpleNpc_0.Config.MinBlood = 1;
			simpleNpc_0.Config.FirstStepMove = int_4;
			simpleNpc_0.MoveTo(list_0[0].X, list_0[0].Y, "walk", 0, 8);
			int_4 -= 2;
			simpleNpc_0 = base.Game.CreateNpc(ByuwzhVvRsi, 100, 1080, 1, 1);
			simpleNpc_0.Config.IsFly = true;
			simpleNpc_0.Config.MaxStepMove = 6;
			simpleNpc_0.Config.MinBlood = 1;
			simpleNpc_0.Config.FirstStepMove = int_4;
			simpleNpc_0.MoveTo(list_0[0].X, list_0[0].Y, "walk", 0, 8);
			int_4 -= 2;
			int_4 = 9;
			simpleNpc_0 = base.Game.CreateNpc(int_1, 50, 1080, 1, 1);
			simpleNpc_0.Config.IsFly = true;
			simpleNpc_0.Config.IsHelper = true;
			simpleNpc_0.Config.MaxStepMove = 4;
			simpleNpc_0.Config.MinBlood = 1;
			simpleNpc_0.Config.FirstStepMove = int_4;
			simpleNpc_0.MoveTo(list_0[0].X, list_0[0].Y, "walk", 0, 8);
			int_4 -= 2;
			simpleNpc_0 = base.Game.CreateNpc(int_1, 0, 1080, 1, 1);
			simpleNpc_0.Config.IsFly = true;
			simpleNpc_0.Config.IsHelper = true;
			simpleNpc_0.Config.MaxStepMove = 4;
			simpleNpc_0.Config.MinBlood = 1;
			simpleNpc_0.Config.FirstStepMove = int_4;
			simpleNpc_0.MoveTo(list_0[0].X, list_0[0].Y, "walk", 0, 8);
			int_4 -= 2;
			simpleNpc_0 = base.Game.CreateNpc(int_1, -50, 1080, 1, 1);
			simpleNpc_0.Config.IsFly = true;
			simpleNpc_0.Config.IsHelper = true;
			simpleNpc_0.Config.MaxStepMove = 4;
			simpleNpc_0.Config.MinBlood = 1;
			simpleNpc_0.Config.FirstStepMove = int_4;
			simpleNpc_0.MoveTo(list_0[0].X, list_0[0].Y, "walk", 0, 8);
			int_4 -= 2;
			simpleNpc_0 = base.Game.CreateNpc(int_1, -100, 1080, 1, 1);
			simpleNpc_0.Config.IsFly = true;
			simpleNpc_0.Config.IsHelper = true;
			simpleNpc_0.Config.MaxStepMove = 4;
			simpleNpc_0.Config.MinBlood = 1;
			simpleNpc_0.Config.FirstStepMove = int_4;
			simpleNpc_0.MoveTo(list_0[0].X, list_0[0].Y, "walk", 0, 8);
			int_4 -= 2;
			simpleNpc_0 = base.Game.CreateNpc(int_1, -150, 1080, 1, 1);
			simpleNpc_0.Config.IsFly = true;
			simpleNpc_0.Config.IsHelper = true;
			simpleNpc_0.Config.MaxStepMove = 4;
			simpleNpc_0.Config.MinBlood = 1;
			simpleNpc_0.Config.FirstStepMove = int_4;
			simpleNpc_0.MoveTo(list_0[0].X, list_0[0].Y, "walk", 0, 8);
			int_4 -= 2;
		}

		public override bool CanGameOver()
		{
			base.CanGameOver();
			SimpleNpc[] array = base.Game.FindAllNpcLiving();
			int_5 = 0;
			int_6 = 0;
			SimpleNpc[] array2 = array;
			foreach (SimpleNpc simpleNpc in array2)
			{
				if (simpleNpc.Config.CompleteStep)
				{
					if (simpleNpc.NpcInfo.ID == int_1)
					{
						int_5++;
					}
					else
					{
						int_6++;
					}
				}
			}
			if (int_5 < 5 && int_6 < 5)
			{
				return false;
			}
			return true;
		}

		public override int UpdateUIData()
		{
			return base.Game.TotalKillCount;
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (int_5 >= 5)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
		}

		public DTGT1166()
		{

			int_0 = 6123;
			ByuwzhVvRsi = 6122;
			int_1 = 6121;
			int_2 = 6124;
			int_3 = 6114;
			int_4 = 11;
			list_0 = new List<Point>
			{
				new Point(620, 1080)
			};

		}
	}
}
