using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
	public class SixHardThirdBadNpcAi : ABrain
	{
		private int int_0;

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
			foreach (PhysicalObj item in list_0)
			{
				base.Game.RemovePhysicalObj(item, sendToClient: true);
			}
			list_0 = new List<PhysicalObj>();
		}

		public override void OnCreated()
		{
			base.OnCreated();
			base.Body.MaxBeatDis = 200;
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			int_0++;
		}

		public override void OnDie()
		{
			base.OnDie();
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		public override void OnAfterTakedBomb()
		{
			base.OnAfterTakedBomb();
		}

		public SixHardThirdBadNpcAi()
		{
			
			list_0 = new List<PhysicalObj>();
			
		}
	}
}
