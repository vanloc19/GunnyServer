using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
	public class SixNormalSecondBoss : ABrain
	{
		private int int_0;

		private int int_1;

		private SimpleNpc[] simpleNpc_0;

		private SimpleNpc[] simpleNpc_1;

		private int int_2;

		private List<PhysicalObj> list_0;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			m_body.CurrentDamagePlus = 1f;
			m_body.CurrentShootMinus = 1f;
		}

		public override void OnCreated()
		{
			base.OnCreated();
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			if (simpleNpc_0 == null || simpleNpc_1 == null)
			{
				simpleNpc_0 = base.Game.GetNPCLivingWithID(int_1);
				simpleNpc_1 = base.Game.GetNPCLivingWithID(int_0);
			}
			SimpleNpc[] array = simpleNpc_0;
			int num = 0;
			while (true)
			{
				if (num < array.Length)
				{
					if (array[num].Blood <= 1)
					{
						break;
					}
					num++;
					continue;
				}
				base.Body.PlayMovie("beatA", 1000, 0);
				(base.Game as PVEGame).SendFreeFocus(1100, 650, 1, 2500, 3000);
				base.Body.CallFuction(method_0, 3500);
				return;
			}
			base.Body.PlayMovie("beat", 1000, 0);
			(base.Game as PVEGame).SendFreeFocus(1100, 650, 1, 2500, 3000);
			base.Body.CallFuction(method_2, 3500);
		}

		private void method_0()
		{
			SimpleNpc[] array = simpleNpc_1;
			foreach (SimpleNpc simpleNpc in array)
			{
				if (!simpleNpc.Config.CompleteStep)
				{
					list_0.Add((base.Game as PVEGame).Createlayer(simpleNpc.X, simpleNpc.Y, "front", "asset.game.six.qunti", "", 1, 0));
				}
			}
			base.Body.CallFuction(method_1, 500);
		}

		private void method_1()
		{
			SimpleNpc[] array = simpleNpc_1;
			foreach (SimpleNpc simpleNpc in array)
			{
				if (!simpleNpc.Config.CompleteStep)
				{
					base.Body.BeatDirect(simpleNpc, "", 100, 1, 1);
				}
			}
		}

		private void method_2()
		{
			SimpleNpc[] array = simpleNpc_0;
			foreach (SimpleNpc simpleNpc in array)
			{
				if (!simpleNpc.Config.CompleteStep)
				{
					list_0.Add((base.Game as PVEGame).Createlayer(simpleNpc.X, simpleNpc.Y, "front", "asset.game.six.qunjia", "", 1, 0));
				}
			}
			base.Body.CallFuction(method_3, 500);
		}

		private void method_3()
		{
			SimpleNpc[] array = simpleNpc_0;
			foreach (SimpleNpc simpleNpc in array)
			{
				if (!simpleNpc.Config.CompleteStep)
				{
					simpleNpc.AddBlood(int_2);
				}
			}
		}

		public override void OnDie()
		{
			base.OnDie();
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		public SixNormalSecondBoss()
		{

			int_0 = 6121;
			int_1 = 6122;
			int_2 = 1000;
			list_0 = new List<PhysicalObj>();

		}
	}
}
