using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;

namespace GameServerScript.AI.Messions
{
	public class WAH13202 : AMissionControl
	{
		private int int_0;

		private int int_1;

		private int int_2;

		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

		private PhysicalObj physicalObj_2;

		private SimpleBoss simpleBoss_0;

		private SimpleNpc simpleNpc_0;

		private SimpleBoss simpleBoss_1;

		public WAH13202()
		{
			
			this.int_0 = 13205;
			this.int_1 = 13203;
			this.int_2 = 13204;
			
		}

		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			if (score > 600)
			{
				return 3;
			}
			if (score > 520)
			{
				return 2;
			}
			if (score > 450)
			{
				return 1;
			}
			return 0;
		}

		public override bool CanGameOver()
		{
			base.CanGameOver();
			if (this.simpleBoss_0 != null && !this.simpleBoss_0.IsLiving)
			{
				return true;
			}
			if (base.Game.TotalTurn > 200)
			{
				return true;
			}
			return false;
		}

		private void method_0()
		{
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.CanTakeDamage = false;
			livingConfig.IsFly = true;
			livingConfig.IsTurn = false;
			this.simpleNpc_0 = base.Game.CreateNpc(this.int_1, 701, 594, 2, 1, "", livingConfig);
			this.simpleNpc_0.SetRelateDemagemRect(this.simpleNpc_0.NpcInfo.X, this.simpleNpc_0.NpcInfo.Y, this.simpleNpc_0.NpcInfo.Width, this.simpleNpc_0.NpcInfo.Height);
			base.Game.SendHideBlood(this.simpleNpc_0, 0);
			base.Game.SendObjectFocus(this.simpleNpc_0, 0, 0, 0);
			this.simpleBoss_0.CallFuction(new LivingCallBack(this.method_1), 2000);
		}

		private void method_1()
		{
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.CanTakeDamage = false;
			livingConfig.IsFly = true;
			this.simpleBoss_1 = base.Game.CreateBoss(this.int_2, 1604, 594, -1, 2, "", livingConfig);
			this.simpleBoss_1.SetRelateDemagemRect(this.simpleBoss_1.NpcInfo.X, this.simpleBoss_1.NpcInfo.Y, this.simpleBoss_1.NpcInfo.Width, this.simpleBoss_1.NpcInfo.Height);
			this.simpleBoss_1.Delay = base.Game.GetHighDelayTurn() + 1;
			base.Game.SendHideBlood(this.simpleBoss_1, 0);
			base.Game.SendObjectFocus(this.simpleBoss_1, 0, 0, 0);
			base.Game.SendFreeFocus(1160, 860, 0, 2000, 0);
			this.physicalObj_0.PlayMovie("in", 4000, 0);
			this.physicalObj_1.PlayMovie("in", 4200, 0);
			this.physicalObj_0.PlayMovie("out", 7000, 0);
			this.physicalObj_1.PlayMovie("out", 7200, 0);
		}

		private void method_2()
		{
			base.Game.SendObjectFocus(this.simpleNpc_0, 1, 3000, 0);
			this.simpleNpc_0.Say("Có lẽ nào đây là giá phải trả cho nghi thức tà thần?", 0, 4000);
			this.simpleNpc_0.PlayMovie("die", 6000, 0);
			this.simpleNpc_0.CallFuction(new LivingCallBack(this.method_3), 7700);
			base.Game.SendObjectFocus(this.simpleBoss_1, 1, 8000, 0);
			this.simpleBoss_1.Say("Các ngươi hãy đợi đấy. Ta sẽ còn quay lại..", 1, 9000);
			this.simpleBoss_1.PlayMovie("die", 12000, 3000);
			this.simpleBoss_1.CallFuction(new LivingCallBack(this.method_4), 13700);
		}

		private void method_3()
		{
			base.Game.RemoveLivings(this.simpleNpc_0.Id);
		}

		private void method_4()
		{
			base.Game.RemoveLivings(this.simpleBoss_1.Id);
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			if (this.physicalObj_0 != null && this.physicalObj_1 != null)
			{
				base.Game.RemovePhysicalObj(this.physicalObj_0, true);
				base.Game.RemovePhysicalObj(this.physicalObj_1, true);
				this.physicalObj_0 = null;
				this.physicalObj_1 = null;
			}
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (this.simpleBoss_0 != null && !this.simpleBoss_0.IsLiving)
			{
				base.Game.IsWin = true;
				return;
			}
			base.Game.IsWin = false;
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
		}

		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			int[] int0 = new int[] { this.int_0, this.int_2, this.int_1 };
			base.Game.AddLoadingFile(1, "bombs/55.swf", "tank.resource.bombs.Bomb55");
			base.Game.AddLoadingFile(2, "image/game/effect/10/tuteng.swf", "asset.game.ten.baozha");
			base.Game.AddLoadingFile(2, "image/game/effect/10/tuteng.swf", "asset.game.ten.jiaodu");
			base.Game.AddLoadingFile(2, "image/game/effect/10/tuteng.swf", "asset.game.ten.pilao");
			base.Game.AddLoadingFile(2, "image/game/effect/10/jitan.swf", "asset.game.ten.jitan");
			base.Game.AddLoadingFile(2, "image/game/effect/10/gongfang.swf", "asset.game.ten.down");
			base.Game.AddLoadingFile(2, "image/game/effect/10/gongfang.swf", "asset.game.ten.up");
			base.Game.AddLoadingFile(2, "image/game/effect/10/zhuzi.swf", "asset.game.ten.zhuzi");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.ClanLeaderAsset");
			base.Game.LoadResources(int0);
			base.Game.LoadNpcGameOverResources(int0);
			base.Game.SetMap(1215);
		}

		public override void OnShooted()
		{
			base.OnShooted();
			if (this.simpleBoss_0 != null && !this.simpleBoss_0.IsLiving)
			{
				int waitTimerLeft = base.Game.GetWaitTimerLeft();
				this.simpleBoss_0.CallFuction(new LivingCallBack(this.method_2), waitTimerLeft);
			}
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			this.physicalObj_2 = base.Game.Createlayer(1150, 1041, "normal", "asset.game.ten.zhuzi", "1", 1, 0);
			this.physicalObj_0 = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
			this.physicalObj_1 = base.Game.Createlayer(970, 750, "front", "game.asset.living.ClanLeaderAsset", "out", 1, 0);
			this.simpleBoss_0 = base.Game.CreateBoss(this.int_0, 1290, 1013, -1, 1, "");
			this.simpleBoss_0.SetRelateDemagemRect(this.simpleBoss_0.NpcInfo.X, this.simpleBoss_0.NpcInfo.Y, this.simpleBoss_0.NpcInfo.Width, this.simpleBoss_0.NpcInfo.Height);
			base.Game.SendObjectFocus(this.simpleBoss_0, 0, 0, 0);
			this.simpleBoss_0.CallFuction(new LivingCallBack(this.method_0), 2000);
		}

		public override int UpdateUIData()
		{
			return base.Game.TotalKillCount;
		}
	}
}