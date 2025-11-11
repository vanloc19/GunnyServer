//using Game.Logic;
//using Game.Logic.AI;
//using Game.Logic.Effects;
//using Game.Logic.Phy.Object;
//using System.Collections.Generic;

//namespace GameServerScript.AI.NPC
//{
//	public class TwelveSimpleFlyThirdBoss : ABrain
//	{
//		private int turn = 1;

//		private int reduceBlood = 1000;

//		#region Lời thoại của Bác học Vẹt
//		private static string[] ElectricChat = new string[] {
//			"Nguồn điện từ 10 ngàn volt",

//			"Điện từ trường cấp 10",

//			"Cảm giác khó tả khi bị giựt điện phải không"
//		};

//		private static string[] HoleChat = new string[]{
//			"Lỗ hổng không gian",

//			"Lốc xoáy vũ trụ"
//		};

//		private static string[] DiamondChat = new string[]{
//			"Ăn này !!!!!!",

//			"Hút này"
//		};

//		private static string[] VoltTakeChat = new string[]{
//			"Nếm thử chiêu này xem !!!",

//			"Cận thận chứ"
//		};
//        #endregion
//        private int[] boltMoveXs = new int[]
//		{
//			570,
//			585,
//			615,
//			685,
//			730,
//			790,
//			820,
//			890,
//			940,
//			1010,
//			1300,
//			1445
//		};

//		public override void OnBeginNewTurn()
//		{
//			base.OnBeginNewTurn();
//			base.Body.CurrentDamagePlus = 1f;
//			base.Body.CurrentShootMinus = 1f;
//		}

//		public override void OnBeginSelfTurn()
//		{
//			base.OnBeginSelfTurn();
//		}

//		public override void OnCreated()
//		{
//			base.OnCreated();
//		}

//		private void BlackHoleAttack()
//		{
//			base.Body.MoveTo(base.Game.Random.Next(675, 1415), base.Game.Random.Next(415, 450), "fly", 6, "", 100, HoleAttack, 100);
//		}

//		private void HoleAttack()
//		{
//			base.Body.Say(HoleChat[Game.Random.Next(0, HoleChat.Length - 1)], 0, 100, 2000);
//			base.Body.PlayMovie("beatD", 100, 6000);
//			base.Body.CallFuction(RotatePlayers, 3000);
//		}

//		private void ElectrcAttack()
//		{
//			base.Body.MoveTo(base.Game.Random.Next(675, 1415), base.Game.Random.Next(415, 450), "fly", 6, "", 100, PreElectricAttack, 100);
//		}

//		private void PreElectricAttack()
//		{
//			Player randomPlayer = base.Game.FindRandomPlayer();
//			base.Body.Say(ElectricChat[Game.Random.Next(0, ElectricChat.Length - 1)], 0, 100, 2000);
//			base.Body.MoveTo(randomPlayer.X, 570, "fly", 6, "", 100, delegate
//			{
//				ElectricAttackPlayer(randomPlayer);
//			}, 100);
//		}

//		private void PearlAttack()
//		{
//			base.Body.MoveTo(base.Game.Random.Next(675, 1415), base.Game.Random.Next(415, 450), "fly", 6, "", 100, PearlAttackPlayer, 100);
//		}

//		private void PearlAttackPlayer()
//		{
//			base.Body.Say(DiamondChat[Game.Random.Next(0, DiamondChat.Length - 1)], 0, 100, 2000);
//			base.Body.PlayMovie("beatC", 100, 6000);
//			((PVEGame)base.Game).SendObjectFocus(base.Game.FindRandomPlayer(), 1, 1500, 100);
//			base.Body.CallFuction(CreateFengyinffect, 3000);
//		}

//		private void ThunderAttack()
//		{
//			base.Body.MoveTo(base.Game.Random.Next(675, 1415), base.Game.Random.Next(415, 450), "fly", 6, "", 100, ThunderReadyAttack, 100);
//		}

//		private void ThunderReadyAttack()
//		{
//			base.Body.Say(VoltTakeChat[Game.Random.Next(0, VoltTakeChat.Length - 1)], 0, 100, 2000);
//			base.Body.PlayMovie("beatA", 100, 6000);
//			((PVEGame)base.Game).SendObjectFocus(base.Game.FindRandomPlayer(), 1, 3000, 100);
//			base.Body.CallFuction(BeatPlayers, 3000);
//		}

//		private void BeatPlayers()
//		{
//			base.Body.CurrentDamagePlus = 25f;
//			foreach (Player allLivingPlayersByProperty in base.Game.GetAllLivingPlayersByProperties(2))
//			{
//				base.Body.BeatDirect(allLivingPlayersByProperty, "", 1, 1, 1);
//				allLivingPlayersByProperty.Properties1 = 0;
//				((PVEGame)base.Game).SendPlayersPicture(allLivingPlayersByProperty, 7, state: false);
//				allLivingPlayersByProperty.SetSeal(state: false);
//			}
//		}

//		private void CreateFengyinffect()
//		{
//			List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
//			if (allLivingPlayers.Count == 1)
//			{
//				((PVEGame)base.Game).Createlayer(allLivingPlayers[0].X, allLivingPlayers[0].Y, "", "asset.game.nine.fengyin", "", 1, 0, CanPenetrate: false);
//				((PVEGame)base.Game).SendPlayersPicture(allLivingPlayers[0], 7, state: true);
//				allLivingPlayers[0].Seal(allLivingPlayers[0], 0, 0);
//				return;
//			}
//			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
//			{
//				if (base.Game.Random.Next(100) > 50)
//				{
//					((PVEGame)base.Game).Createlayer(allLivingPlayer.X, allLivingPlayer.Y, "", "asset.game.nine.fengyin", "", 1, 0, CanPenetrate: false);
//					((PVEGame)base.Game).SendPlayersPicture(allLivingPlayer, 7, state: true);
//					allLivingPlayer.Seal(allLivingPlayer, 0, 0);
//					allLivingPlayer.Properties1 = 2;
//				}
//			}
//		}

//		private void ElectricAttackPlayer(Player player)
//		{
//			base.Body.PlayMovie("beatB", 100, 6000);
//			((PVEGame)base.Game).SendFreeFocus(player.X, player.Y - 100, 1, 1, 1);
//			base.Body.CallFuction(delegate
//			{
//				CreateDiancipaoEffect(player);
//			}, 4600);
//		}

//		private void CreateDiancipaoEffect(Player player)
//		{
//			base.Body.CurrentDamagePlus = 10f;
//			((PVEGame)base.Game).Createlayer(player.X, player.Y, "", "asset.game.nine.diancipao", "", 1, 0, CanPenetrate: false);
//			base.Body.BeatDirect(player, "", 1, 1, 1);
//			player.AddEffect(new ContinueReduceBloodEffect(2, reduceBlood, base.Body), 100);
//		}

//		private void RotatePlayers()
//		{
//			Game.Shuffer<int>(boltMoveXs);
//			base.Body.CurrentDamagePlus = 15f;
//			List<int> boltRandY = new List<int>();
//			for (int i = 0; i < 4; i++)
//			{
//				int randY = Game.Random.Next(550, 650);
//				boltRandY.Add(randY);
//				((PVEGame)base.Game).Createlayer(boltMoveXs[i], randY, "", "asset.game.nine.heidong", "in", 1, 0, CanPenetrate: true);
//			}
//			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
//			{
//				base.Body.RangeAttacking(500, 1500, "", 1, null);
//				base.Game.LivingChangeAngle(allLivingPlayer, -2000, -2000, "");
//				int x = boltMoveXs[base.Game.Random.Next(boltMoveXs.Length)];
//				int y = boltMoveXs[base.Game.Random.Next(boltRandY.Count)];
//				allLivingPlayer.BoltMove(x, y, 0);
//			}
//			((PVEGame)base.Game).SendFreeFocus(585, 600, 1, 1, 1);
//			((PVEGame)base.Game).SendFreeFocus(1400, 600, 1, 1000, 1);
//			base.Body.CallFuction(StopRotatePlayers, 3000);
//		}

//		private void StopRotatePlayers()
//		{
//			List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
//			foreach (Player item in allLivingPlayers)
//			{
//				base.Game.LivingChangeAngle(item, 0, 0, "");
//				item.FallFrom(item.X, item.Y, "", 500, 0, 1);
//			}
//		}

//		public override void OnStartAttacking()
//		{
//			base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
//			switch (turn)
//			{
//				case 1:
//					BlackHoleAttack();
//					break;
//				case 2:
//					PearlAttack();
//					break;
//				case 3:
//					ThunderAttack();
//					break;
//				case 4:
//					ElectrcAttack();
//					break;
//				default:
//					turn = 1;
//					goto case 1;
//			}
//			turn++;
//		}

//		public override void OnStopAttacking()
//		{
//			base.OnStopAttacking();
//		}
//	}
//}
using System;
using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	// Token: 0x020001D4 RID: 468
	public class TwelveTerrorFlyThirdBoss : ABrain
	{
		// Token: 0x060016C2 RID: 5826 RVA: 0x00042158 File Offset: 0x00040358
		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		// Token: 0x060016C3 RID: 5827 RVA: 0x0009D88F File Offset: 0x0009BA8F
		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			base.Body.CurrentDamagePlus = 1f;
			base.Body.CurrentShootMinus = 1f;
			this.m_attackTurn++;
		}

		// Token: 0x060016C4 RID: 5828 RVA: 0x00042161 File Offset: 0x00040361
		public override void OnCreated()
		{
			base.OnCreated();
		}

		// Token: 0x060016C5 RID: 5829 RVA: 0x0009D8C8 File Offset: 0x0009BAC8
		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			int x = base.Game.Random.Next(600, 1300);
			int y = base.Game.Random.Next(300, 400);
			bool flag = this.m_attackTurn == 1;
			if (flag)
			{
				base.Body.MoveTo(x, y, "fly", 1000, "fly", 8);
				base.Body.CallFuction(new LivingCallBack(this.HoleAttack), 3000);
			}
			else
			{
				bool flag2 = this.m_attackTurn == 2;
				if (flag2)
				{
					base.Body.MoveTo(x, y, "fly", 1000, "fly", 8);
					base.Body.CallFuction(new LivingCallBack(this.ReadyElectricAttack), 3000);
				}
				else
				{
					bool flag3 = this.m_attackTurn == 3;
					if (flag3)
					{
						base.Body.MoveTo(x, y, "fly", 1000, "fly", 8);
						base.Body.CallFuction(new LivingCallBack(this.ReadyDiamondAttack), 3000);
					}
					else
					{
						base.Body.MoveTo(x, y, "fly", 1000, "fly", 8);
						base.Body.CallFuction(new LivingCallBack(this.ReadySockAttack), 3000);
						this.m_attackTurn = 0;
					}
				}
			}
		}

		// Token: 0x060016C6 RID: 5830 RVA: 0x000422B8 File Offset: 0x000404B8
		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		// Token: 0x060016C7 RID: 5831 RVA: 0x000026F0 File Offset: 0x000008F0
		private void KillAttack(int fx)
		{
		}

		// Token: 0x060016C8 RID: 5832 RVA: 0x000026F0 File Offset: 0x000008F0
		private void AllAttack()
		{
		}

		// Token: 0x060016C9 RID: 5833 RVA: 0x0009DA44 File Offset: 0x0009BC44
		private void ReadySockAttack()
		{
			base.Body.PlayMovie("beatA", 5800, 0);
			base.Body.Say("Hãy chết đi !!!!", 1, 0);
			base.Body.CallFuction(new LivingCallBack(this.SockAttack), 5800);
		}

		// Token: 0x060016CA RID: 5834 RVA: 0x0009DA9C File Offset: 0x0009BC9C
		private void SockAttack()
		{
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				bool isLiving = allFightPlayer.IsLiving;
				if (isLiving)
				{
					base.Body.RangeAttacking(allFightPlayer.X - 200, allFightPlayer.X + 200, "cryA", 2000, null);
					this.Hole = ((PVEGame)base.Game).Createlayer(allFightPlayer.X, allFightPlayer.Y, "moive", "asset.game.nine.dianxipao", "", 1, 0);
				}
			}
		}

		// Token: 0x060016CB RID: 5835 RVA: 0x0009DB64 File Offset: 0x0009BD64
		private void ReadyDiamondAttack()
		{
			base.Body.PlayMovie("beatC", 5800, 0);
			base.Body.Say("Hãy tiếp chiêu !!!!", 1, 0);
			base.Body.CallFuction(new LivingCallBack(this.DiamondAttack), 5800);
		}

		// Token: 0x060016CC RID: 5836 RVA: 0x0009DBBC File Offset: 0x0009BDBC
		private void DiamondAttack()
		{
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				bool isLiving = allFightPlayer.IsLiving;
				if (isLiving)
				{
					base.Body.RangeAttacking(allFightPlayer.X - 200, allFightPlayer.X + 200, "cryA", 2000, null);
					this.Hole = ((PVEGame)base.Game).Createlayer(allFightPlayer.X, allFightPlayer.Y, "moive", "asset.game.nine.fengyin", "", 1, 0);
					this.Hole = ((PVEGame)base.Game).Createlayer(allFightPlayer.X, allFightPlayer.Y, "moive", "asset.game.nine.fengyin", "", 1, 0);
					allFightPlayer.AddEffect(new ContinueReduceBloodEffect(2, 4000, null), 0);
				}
			}
		}

		// Token: 0x060016CD RID: 5837 RVA: 0x0009DCD0 File Offset: 0x0009BED0
		private void ReadyElectricAttack()
		{
			base.Body.PlayMovie("beatB", 6000, 0);
			int index = base.Game.Random.Next(0, TwelveTerrorFlyThirdBoss.ElectricChat.Length);
			base.Body.Say(TwelveTerrorFlyThirdBoss.ElectricChat[index], 1, 0);
			base.Body.CallFuction(new LivingCallBack(this.ElectricAttack), 6000);
		}

		// Token: 0x060016CE RID: 5838 RVA: 0x0009DD40 File Offset: 0x0009BF40
		private void ElectricAttack()
		{
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				bool isLiving = allFightPlayer.IsLiving;
				if (isLiving)
				{
					base.Body.RangeAttacking(allFightPlayer.X - 200, allFightPlayer.X + 200, "cryA", 2000, null);
					this.Hole = ((PVEGame)base.Game).Createlayer(allFightPlayer.X, allFightPlayer.Y, "moive", "asset.game.nine.dianxipao", "", 1, 0);
				}
			}
		}

		// Token: 0x060016CF RID: 5839 RVA: 0x0009DE08 File Offset: 0x0009C008
		private void HoleAttack()
		{
			base.Body.PlayMovie("beatD", 1000, 0);
			int index = base.Game.Random.Next(0, TwelveTerrorFlyThirdBoss.HoleChat.Length);
			base.Body.Say(TwelveTerrorFlyThirdBoss.HoleChat[index], 1, 0);
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				int num = base.Game.Random.Next(3000, 4000);
				int x = base.Game.Random.Next(400, 820);
				int y = base.Game.Random.Next(400, 600);
				base.Body.CurrentDamagePlus = 1f;
				this.Hole = ((PVEGame)base.Game).Createlayer(x, y, "moive", "asset.game.nine.heidong", "", 1, 0);
				allFightPlayer.SetXY(this.Hole.X, this.Hole.Y);
				base.Body.RangeAttacking(allFightPlayer.X - 200, allFightPlayer.X + 200, "cryA", 2000, null);
				allFightPlayer.ReducedBlood(-num);
			}
		}

		// Token: 0x04000C63 RID: 3171
		private int m_attackTurn = 0;

		// Token: 0x04000C64 RID: 3172
		public int currentCount = 0;

		// Token: 0x04000C65 RID: 3173
		public int Dander = 0;

		// Token: 0x04000C66 RID: 3174
		private PhysicalObj Hole;

		// Token: 0x04000C67 RID: 3175
		private int npcID = 1009;

		// Token: 0x04000C68 RID: 3176
		protected List<Living> m_livings;

		// Token: 0x04000C69 RID: 3177
		public List<SimpleNpc> Children = new List<SimpleNpc>();

		// Token: 0x04000C6A RID: 3178
		private static string[] ElectricChat = new string[]
		{
			"Nguồn điện từ 10 ngàn volt",
			"Điện từ trường cấp 10",
			"Cảm giác khó tả khi bị giựt điện phải không"
		};

		// Token: 0x04000C6B RID: 3179
		private static string[] HoleChat = new string[]
		{
			"Lỗ hổng không gian",
			"Lốc xoáy vũ trụ"
		};

		// Token: 0x04000C6C RID: 3180
		private static string[] DiamondChat = new string[]
		{
			"Ăn này !!!!!!",
			"Hút này"
		};

		// Token: 0x04000C6D RID: 3181
		private static string[] VoltTakeChat = new string[]
		{
			"Nếm thử chiêu này xem !!!",
			"Cận thận chứ"
		};

		// Token: 0x04000C6E RID: 3182
		private static string[] ShootedChat = new string[]
		{
			"Hiha…",
			"Hello mấy bạn…"
		};

		// Token: 0x04000C6F RID: 3183
		private static string[] JumpChat = new string[]
		{
			"Gì ó！",
			"Hello lại là mình đây！",
			"Ya hoo！"
		};

		// Token: 0x04000C70 RID: 3184
		private static string[] KillAttackChat = new string[]
		{
			"MrD！！"
		};
	}
}

