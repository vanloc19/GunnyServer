using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
	public class SeventhHardHouseAi : ABrain
	{
		private int int_0;

		private List<SimpleNpc> list_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private int int_4;

		private int int_5;

		private int int_6;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			if (int_0 == 0 && base.Body.Blood < base.Body.MaxBlood / 2)
			{
				base.Body.PlayMovie("toA", 0, 1200);
			}
			method_0();
			base.Body.CurrentDamagePlus = 1f;
			base.Body.CurrentShootMinus = 1f;
		}

		public override void OnCreated()
		{
			base.OnCreated();
		}

		public override void OnStartAttacking()
		{
			bool flag = false;
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				if (allFightPlayer.IsLiving && allFightPlayer.X > base.Body.X - 400 && allFightPlayer.X < base.Body.X + 400)
				{
					flag = true;
				}
			}
			if (flag)
			{
				KillAttack(base.Body.X - 400, base.Body.X + 400);
				return;
			}
			int_5 = 1;
			int_6 = 4;
			CreateNpc1();
			CreateNpc2();
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		private void method_0()
		{
			if (int_0 == 0)
			{
				base.Body.PlayMovie("stand", 2000, 0);
			}
			else
			{
				base.Body.PlayMovie("standA", 2000, 0);
			}
		}

		public void KillAttack(int fx, int tx)
		{
			base.Body.RangeAttacking(fx, tx, "cry", 1000, null);
		}

		public void CreateNpc1()
		{
			if (int_5 > int_3)
			{
				int_5 = int_3;
			}
			int num = int_3 - base.Game.GetLivedNpcs(int_2).Count;
			if (int_5 > num)
			{
				int_5 = num;
			}
			if (int_5 > 0)
			{
				for (int i = 0; i < int_5; i++)
				{
					((SimpleBoss)base.Body).CreateChild(int_2, 596, 955, 1, 1, true, ((PVEGame)base.Game).BaseLivingConfig());
				}
			}
		}

		public void CreateNpc2()
		{
			if (int_6 > int_4)
			{
				int_6 = int_4;
			}
			int num = int_4 - base.Game.GetLivedNpcs(int_1).Count;
			if (int_6 > num)
			{
				int_6 = num;
			}
			if (int_6 > 0)
			{
				for (int i = 0; i < int_6; i++)
				{
					((SimpleBoss)base.Body).CreateChild(int_1, 792 + i * 50, 950, 1, 1, true, ((PVEGame)base.Game).BaseLivingConfig());
				}
			}
		}

		public SeventhHardHouseAi()
		{

			list_0 = new List<SimpleNpc>();
			int_1 = 7222;
			int_2 = 7221;
			int_3 = 5;
			int_4 = 15;

		}
	}
}
