using System.Drawing;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;
using GameServerScript.AI.Messions;

namespace GameServerScript.AI.NPC
{
	public class FourHardBlowArmsNpc : ABrain
	{
		private int int_0;

		private PhysicalObj physicalObj_0;

		private SimpleNpc simpleNpc_0;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
			(base.Body.EffectList.GetOfType(eEffectType.IceFronzeEffect) as IceFronzeEffect)?.Stop();
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
			Point point = default(Point);
			switch (int_0)
			{
				case 0:
					point = new Point(672, 746);
					break;
				case 1:
					point = new Point(1059, 749);
					break;
				case 2:
					point = new Point(1412, 751);
					break;
			}
			int num = int.MaxValue;
			SimpleNpc[] array = base.Game.FindAllNpcLiving();
			SimpleNpc[] array2 = array;
			foreach (SimpleNpc simpleNpc in array2)
			{
				if (simpleNpc.IsLiving && simpleNpc.X >= base.Body.X && simpleNpc.X <= base.Body.X + point.X)
				{
					int num2 = (int)base.Body.Distance(simpleNpc.X, simpleNpc.Y);
					if (num2 < num)
					{
						simpleNpc_0 = simpleNpc;
						num = num2;
					}
				}
			}
			if (simpleNpc_0 != null)
			{
				method_0(simpleNpc_0.X - 20, simpleNpc_0.Y, BeatNpc);
			}
			else if (int_0 < 2)
			{
				method_0(point.X, point.Y, null);
			}
			else
			{
				method_0(point.X, point.Y, method_1);
				int_0 = 0;
			}
			int_0++;
		}

		private void method_0(int int_1, int int_2, LivingCallBack livingCallBack_0)
		{
			base.Body.MoveTo(int_1, int_2, "walk", 1000, livingCallBack_0, 5);
		}

		public void BeatNpc()
		{
			base.Body.Beat(simpleNpc_0, "die", 5000, 5000, 800);
			base.Body.Die(3000);
		}

		private void method_1()
		{
			base.Body.PlayMovie("beatA", 2000, 6000);
			base.Body.CallFuction(method_2, 4500);
		}

		private void method_2()
		{
			switch (((PVEGame)base.Game).MissionAI.UpdateUIData())
			{
				case 0:
					if (physicalObj_0 == null)
					{
						physicalObj_0 = ((PVEGame)base.Game).Createlayer(1590, 750, "", "game.asset.Gate", "cryA", 1, 0);
					}
					else
					{
						physicalObj_0.PlayMovie("cryA", 0, 0);
					}
					break;
				case 1:
					if (physicalObj_0 == null)
					{
						physicalObj_0 = ((PVEGame)base.Game).Createlayer(1590, 750, "", "game.asset.Gate", "cryB", 1, 0);
					}
					else
					{
						physicalObj_0.PlayMovie("cryB", 0, 0);
					}
					break;
				case 2:
					if (physicalObj_0 == null)
					{
						physicalObj_0 = ((PVEGame)base.Game).Createlayer(1590, 750, "", "game.asset.Gate", "cryC", 1, 0);
					}
					else
					{
						physicalObj_0.PlayMovie("cryC", 0, 0);
					}
					break;
			}
			(((PVEGame)base.Game).MissionAI as PDHAK1142).CountKill++;
			base.Body.Die();
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}
	}
}
