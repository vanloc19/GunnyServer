using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;
using System.Collections.Generic;
using System.Drawing;

namespace GameServerScript.AI.Messions
{
	public class DTGK1168 : AMissionControl
	{
		private SimpleBoss simpleBoss_0;

		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

		private PhysicalObj physicalObj_2;

		private int int_0;

		private int int_1;

		private int int_2;

		private int yltwegeuTpK;

		private int int_3;

		private int int_4;

		private int int_5;

		private int int_6;

		private PhysicalObj physicalObj_3;

		private PhysicalObj physicalObj_4;

		private PhysicalObj physicalObj_5;

		private static string[] string_0;

		private static string[] string_1;

		private Point[] point_0;

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
			base.Game.AddLoadingFile(1, "bombs/61.swf", "tank.resource.bombs.Bomb61");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.bogucaipanAsset");
			base.Game.AddLoadingFile(2, "image/game/effect/6/chang.swf", "asset.game.six.chang");
			base.Game.AddLoadingFile(2, "image/game/effect/6/bluecircle.swf", "asset.game.six.bluecircle");
			base.Game.AddLoadingFile(2, "image/game/effect/6/greencircle.swf", "asset.game.six.greencircle");
			base.Game.AddLoadingFile(2, "image/game/effect/6/redcircle.swf", "asset.game.six.redcircle");
			int[] npcIds = new int[3]
			{
				int_1,
				int_2,
				yltwegeuTpK
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds);
			base.Game.SetMap(1168);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			physicalObj_2 = base.Game.Createlayer(1250, 520, "moive", "game.living.Living189", "", 1, 0);
			physicalObj_1 = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
			physicalObj_0 = base.Game.Createlayer(1081, 700, "front", "game.asset.living.bogucaipanAsset", "out", 1, 0);
			simpleBoss_0 = base.Game.CreateBoss(int_1, 1250, 1000, -1, 1, "born");
			simpleBoss_0.FallFrom(simpleBoss_0.X, simpleBoss_0.Y, "", 0, 0, 1000);
			simpleBoss_0.SetRect(-70, -110, 130, 116);
			simpleBoss_0.SetRelateDemagemRect(-10, -80, 30, 90);
			base.Game.SendGameFocus(1245, 520, 1, 0, 3000);
			simpleBoss_0.Say("Cái lũ óc cún này dám giả dạng à? Ta sẽ cho các ngươi biết tay.", 0, 3100);
			simpleBoss_0.PlayMovie("shengqi", 1000, 2000);
			simpleBoss_0.PlayMovie("xialai", 2000, 3000);
			physicalObj_1.PlayMovie("in", 6000, 0);
			physicalObj_0.PlayMovie("in", 6100, 0);
			physicalObj_1.PlayMovie("out", 9000, 0);
			physicalObj_0.PlayMovie("out", 9100, 0);
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			if (base.Game.CurrentTurnLiving is SimpleBoss)
			{
				if (physicalObj_3 != null && int_3 > 0)
				{
					int_3--;
					method_1();
				}
				else
				{
					method_0();
				}
			}
			int_0 = 0;
		}

		private void method_0()
		{
			if (physicalObj_3 != null)
			{
				base.Game.RemovePhysicalObj(physicalObj_3, true);
				base.Game.RemovePhysicalObj(physicalObj_4, true);
				base.Game.RemovePhysicalObj(physicalObj_5, true);
			}
			List<Point> list = new List<Point>();
			Point[] array = point_0;
			foreach (Point item in array)
			{
				list.Add(item);
			}
			int index = base.Game.Random.Next(list.Count);
			physicalObj_3 = base.Game.Createlayer(list[index].X, list[index].Y, "moive", "asset.game.six.greencircle", "", 1, 0);
			list.RemoveAt(index);
			index = base.Game.Random.Next(list.Count);
			physicalObj_4 = base.Game.Createlayer(list[index].X, list[index].Y, "moive", "asset.game.six.bluecircle", "", 1, 0);
			list.RemoveAt(index);
			index = base.Game.Random.Next(list.Count);
			physicalObj_5 = base.Game.Createlayer(list[index].X, list[index].Y, "moive", "asset.game.six.redcircle", "", 1, 0);
		}

		private void method_1()
		{
			if (physicalObj_3 != null && physicalObj_4 != null && physicalObj_5 != null)
			{
				List<Player> list = new List<Player>();
				foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
				{
					if (allFightPlayer.IsLiving && allFightPlayer.X > physicalObj_3.X - 100 && allFightPlayer.X < physicalObj_3.X + 100)
					{
						list.Add(allFightPlayer);
					}
				}
				if (list.Count > 0)
				{
					foreach (Player item in list)
					{
						item.AddEffect(new AddBloodTurnEffect(int_6, 1), 0);
					}
				}
				list = new List<Player>();
				foreach (Player allFightPlayer2 in base.Game.GetAllFightPlayers())
				{
					if (allFightPlayer2.IsLiving && allFightPlayer2.X > physicalObj_4.X - 100 && allFightPlayer2.X < physicalObj_4.X + 100)
					{
						list.Add(allFightPlayer2);
					}
				}
				if (list.Count > 0)
				{
					foreach (Player item2 in list)
					{
						item2.AddEffect(new AddGuardEquipEffect(int_5, 1, isArrmor: false), 0);
					}
				}
				list = new List<Player>();
				foreach (Player allFightPlayer3 in base.Game.GetAllFightPlayers())
				{
					if (allFightPlayer3.IsLiving && allFightPlayer3.X > physicalObj_5.X - 100 && allFightPlayer3.X < physicalObj_5.X + 100)
					{
						list.Add(allFightPlayer3);
					}
				}
				if (list.Count > 0)
				{
					foreach (Player item3 in list)
					{
						item3.AddEffect(new AddDamageTurnEffect(int_4, 1), 0);
					}
				}
			}
		}

		public override bool CanGameOver()
		{
			base.CanGameOver();
			if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
			{
				return true;
			}
			if (!simpleBoss_0.IsLiving)
			{
				return true;
			}
			return false;
		}

		public override int UpdateUIData()
		{
			if (simpleBoss_0 == null)
			{
				return 0;
			}
			if (!simpleBoss_0.IsLiving)
			{
				return 1;
			}
			return base.UpdateUIData();
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (!simpleBoss_0.IsLiving)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
		}

		public DTGK1168()
		{

			int_1 = 6241;
			int_2 = 6243;
			yltwegeuTpK = 6244;
			int_3 = 4;
			int_4 = 100;
			int_5 = 20;
			int_6 = 1000;
			point_0 = new Point[4]
			{
				new Point(663, 1000),
				new Point(876, 1000),
				new Point(1267, 1000),
				new Point(1653, 1000)
			};

		}

		static DTGK1168()
		{

			string_0 = new string[2]
			{
				"Gửi cho bạn trở về nhà!",
				"Một mình, bạn có ảo tưởng có thể đánh bại tôi?"
			};
			string_1 = new string[2]
			{
				" Đau ah! Đau ...",
				"Quốc vương vạn tuế ..."
			};
		}
	}
}
