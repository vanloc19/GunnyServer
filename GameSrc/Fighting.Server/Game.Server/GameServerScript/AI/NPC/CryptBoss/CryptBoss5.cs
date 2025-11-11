using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
	public class CryptBoss5 : ABrain
	{
		private int int_0;

		public int currentCount;

		public int Dander;

		private PhysicalObj physicalObj_0;

		private Player player_0;

		public CryptBoss5()
		{
			
			
		}

		private void method_0(int int_1, int int_2)
		{
			base.Body.CurrentDamagePlus = 2000.5f;
			base.Body.PlayMovie("beatB", 1000, 0);
			base.Body.RangeAttacking(int_1, int_2, "cry", 3000, null);
		}

		private void method_1()
		{
			base.Body.CurrentDamagePlus = 2.5f;
			base.Body.PlayMovie("beatA", 500, 0);
			base.Body.CallFuction(new LivingCallBack(this.method_2), 2500);
		}

		private void method_2()
		{
			base.Body.RangeAttacking(0, base.Game.Map.Info.ForegroundWidth + 1, "cry", 0, null);
		}

		private void method_3()
		{
			base.Body.PlayMovie("beatC", 500, 0);
			base.Body.CallFuction(new LivingCallBack(this.method_4), 3500);
		}

		private void method_4()
		{
			Player[] allPlayers = base.Game.GetAllPlayers();
			for (int i = 0; i < (int)allPlayers.Length; i++)
			{
				Player player = allPlayers[i];
				if (player.X > 200)
				{
					player.StartSpeedMult(player.X - 200, player.Y, 0);
				}
			}
			base.Body.CallFuction(new LivingCallBack(this.method_5), 1000);
		}

		private void method_5()
		{
			base.Body.CurrentDamagePlus = 1.9f;
			base.Body.RangeAttacking(0, base.Game.Map.Info.ForegroundWidth + 1, "cry", 0, null);
		}

		private void method_6()
		{
			base.Body.PlayMovie("beatB", 700, 0);
			base.Body.CallFuction(new LivingCallBack(this.method_7), 3000);
		}

		private void method_7()
		{
			this.player_0 = base.Game.FindRandomPlayer();
			if (this.player_0 != null)
			{
				((PVEGame)base.Game).SendGameFocus(this.player_0, 0, 1000);
				this.physicalObj_0 = ((PVEGame)base.Game).Createlayer(this.player_0.X, this.player_0.Y, "moive", "asset.game.eleven.057", "out", 1, 1);
				base.Body.CallFuction(new LivingCallBack(this.method_5), 1000);
				base.Body.CallFuction(new LivingCallBack(this.method_8), 2000);
			}
		}

		private void method_8()
		{
			if (this.physicalObj_0 != null)
			{
				base.Game.RemovePhysicalObj(this.physicalObj_0, true);
				this.physicalObj_0 = null;
			}
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			base.Body.CurrentDamagePlus = 1f;
			base.Body.CurrentShootMinus = 1f;
			base.Body.SetRect(((SimpleBoss)base.Body).NpcInfo.X, ((SimpleBoss)base.Body).NpcInfo.Y, ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Height);
			if (base.Body.Direction == -1)
			{
				base.Body.SetRect(((SimpleBoss)base.Body).NpcInfo.X, ((SimpleBoss)base.Body).NpcInfo.Y, ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Height);
				return;
			}
			base.Body.SetRect(-((SimpleBoss)base.Body).NpcInfo.X - ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Y, ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Height);
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
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				if (!allFightPlayer.IsLiving || allFightPlayer.X <= base.Body.X - 150 || allFightPlayer.X >= base.Body.X + 150)
				{
					continue;
				}
				flag = true;
			}
			if (flag)
			{
				this.method_0(base.Body.X - 150, base.Body.X + 150);
				return;
			}
			if (this.int_0 == 0)
			{
				this.method_1();
				this.int_0++;
				return;
			}
			if (this.int_0 != 1)
			{
				this.method_3();
				this.int_0 = 0;
				return;
			}
			this.method_6();
			this.int_0++;
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}
	}
}