using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class GCGCK1163 : AMissionControl
	{
		private SimpleBoss simpleBoss_0;

		private int int_0;

		private int int_1;

		private int int_2;

		private int int_3;

		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			if (score > 1750)
			{
				return 3;
			}
			if (score > 1675)
			{
				return 2;
			}
			if (score > 1600)
			{
				return 1;
			}
			return 0;
		}

		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			int[] npcIds = new int[3]
			{
				int_2,
				int_0,
				int_1
			};
			int[] npcIds2 = new int[1]
			{
				int_2
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.AddLoadingFile(1, "bombs/84.swf", "tank.resource.bombs.Bomb84");
			base.Game.AddLoadingFile(2, "image/game/effect/7/cao.swf", "asset.game.seven.cao");
			base.Game.SetMap(1163);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			simpleBoss_0 = base.Game.CreateBoss(int_2, 275, 950, 1, 1, "");
			simpleBoss_0.FallFrom(338, 950, "", 0, 0, 2000);
			simpleBoss_0.SetRelateDemagemRect(simpleBoss_0.NpcInfo.X, simpleBoss_0.NpcInfo.Y, simpleBoss_0.NpcInfo.Width, simpleBoss_0.NpcInfo.Height);
			simpleBoss_0.Say("Đừng để bọn chúng tiếp tục xâm nhập!", 0, 2000, 4000);
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		public override bool CanGameOver()
		{
			if (simpleBoss_0 != null && !simpleBoss_0.IsLiving)
			{
				int_3++;
				return true;
			}
			return false;
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return int_3;
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (simpleBoss_0 != null && !simpleBoss_0.IsLiving)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
		}

		public GCGCK1163()
		{

			int_0 = 7221;
			int_1 = 7222;
			int_2 = 7223;

		}
	}
}
