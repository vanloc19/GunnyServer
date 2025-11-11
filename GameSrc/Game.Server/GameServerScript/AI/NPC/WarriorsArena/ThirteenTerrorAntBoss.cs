using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
	public class ThirteenTerrorAntBoss : ABrain
	{
		private int osNsAhZrnc6;

		private SimpleBoss simpleBoss_0;

		protected Player m_targer;

		private List<PhysicalObj> list_0;

		private int int_0;

		private string[] string_0;

		private string[] string_1;

		public ThirteenTerrorAntBoss()
		{
			
			this.list_0 = new List<PhysicalObj>();
			this.int_0 = 13302;
			this.string_0 = new string[] { "Hãy liếm thử mũi tên này của ta", "Xem có thốn không nhé", "Ta sẽ hạ sát hết lũ các ngươi", "Các ngươi nghĩ đủ trình hạ ta?." };
			this.string_1 = new string[] { "Băng trâm tiễn.", "Liếm thử băng trâm tiễn của ta", "Để ta giúp ngươi về trời nhanh." };
			
		}

		private void method_0()
		{
			base.Body.CurrentDamagePlus = 1.3f;
			this.m_targer = base.Game.FindRandomPlayer();
			if (base.Body.ShootPoint(this.m_targer.X, this.m_targer.Y, 51, 1000, 10000, 1, 3f, 2800))
			{
				base.Body.PlayMovie("beatD", 1000, 3000);
			}
		}

		private void method_1()
		{
			int num = base.Game.Random.Next(0, (int)this.string_1.Length);
			base.Body.Say(this.string_1[num], 0, 1000, 0);
			Player[] playerArray = base.Game.FindRandomPlayer(2);
			if (playerArray.Length != 0)
			{
				if (base.Body.ShootPoint(playerArray[0].X - 20, playerArray[0].Y, 99, 1000, 10000, 1, 3f, 3000))
				{
					base.Body.PlayMovie("beatD", 1500, 3000);
				}
				if ((int)playerArray.Length == 2 && base.Body.ShootPoint(playerArray[1].X - 20, playerArray[1].Y, 99, 1000, 10000, 1, 3f, 5000))
				{
					base.Body.PlayMovie("beatD", 3500, 3000);
				}
			}
		}

		private void method_2()
		{
			base.Body.CurrentDamagePlus = 1.5f;
			int num = base.Game.Random.Next(0, (int)this.string_0.Length);
			base.Body.Say(this.string_0[num], 0, 1000, 6000);
			base.Body.PlayMovie("beatC", 1500, 0);
			this.m_targer = base.Game.FindRandomPlayer();
			(base.Game as PVEGame).SendObjectFocus(this.m_targer, 0, 3000, 0);
			base.Body.CallFuction(new LivingCallBack(this.method_3), 4000);
		}

		private void method_3()
		{
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				this.list_0.Add(((PVEGame)base.Game).Createlayer(allLivingPlayer.X, allLivingPlayer.Y, "", "asset.game.ten.jianyu", "", 1, 1));
				base.Body.BeatDirect(allLivingPlayer, "", 2000, 3, 1);
			}
		}

		private void method_4(List<Player> hTU7wiCP64lHjuMffoj)
		{
			base.Body.CurrentDamagePlus = 1000f;
			base.Body.PlayMovie("beatB", 1000, 0);
			foreach (Player player in hTU7wiCP64lHjuMffoj)
			{
				base.Body.BeatDirect(player, "", 2500, 1, 1);
			}
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			this.m_body.CurrentDamagePlus = 1f;
			this.m_body.CurrentShootMinus = 1f;
			if (this.list_0.Count > 0)
			{
				foreach (PhysicalObj list0 in this.list_0)
				{
					base.Game.RemovePhysicalObj(list0, true);
				}
				this.list_0.Clear();
			}
		}

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
			if (this.simpleBoss_0 == null)
			{
				SimpleBoss[] simpleBossArray = ((PVEGame)base.Game).FindLivingTurnBossWithID(this.int_0);
				if (simpleBossArray.Length != 0)
				{
					this.simpleBoss_0 = simpleBossArray[0];
				}
			}
		}

		public override void OnCreated()
		{
			base.OnCreated();
		}

		public override void OnDie()
		{
			base.OnDie();
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			List<Player> players = new List<Player>();
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				if (!allLivingPlayer.IsLiving || allLivingPlayer.X <= 1066)
				{
					continue;
				}
				players.Add(allLivingPlayer);
			}
			if (players.Count > 0)
			{
				this.method_4(players);
				return;
			}
			switch (this.osNsAhZrnc6)
			{
				case 0:
				{
					this.method_2();
					break;
				}
				case 1:
				{
					this.method_1();
					break;
				}
				case 2:
				{
					this.method_0();
					this.osNsAhZrnc6 = -1;
					break;
				}
			}
			this.osNsAhZrnc6++;
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}
	}
}