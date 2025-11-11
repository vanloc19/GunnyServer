using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class SeventhNormalMaleAi : ABrain
	{
		protected Player m_targer;

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
			m_targer = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
			Beating();
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		public void MoveToPlayer(Player player)
		{
			int num = (int)player.Distance(base.Body.X, base.Body.Y);
			int num2 = base.Game.Random.Next(((SimpleNpc)base.Body).NpcInfo.MoveMin, ((SimpleNpc)base.Body).NpcInfo.MoveMax);
			if (num <= 97)
			{
				return;
			}
			num = ((num <= ((SimpleNpc)base.Body).NpcInfo.MoveMax) ? (num - 90) : num2);
			if (player.Y < 420 && player.X < 210)
			{
				if (base.Body.Y > 420)
				{
					base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, MoveBeat);
				}
				else if (player.X > base.Body.X)
				{
					base.Body.MoveTo(base.Body.X + num, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, MoveBeat);
				}
				else
				{
					base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, MoveBeat);
				}
			}
			else if (base.Body.Y < 420)
			{
				if (base.Body.X + num > 200)
				{
					base.Body.MoveTo(200, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, Fall);
				}
			}
			else if (player.X > base.Body.X)
			{
				base.Body.MoveTo(base.Body.X + num, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, MoveBeat);
			}
			else
			{
				base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, MoveBeat);
			}
		}

		public void MoveBeat()
		{
			base.Body.Beat(m_targer, "beat", 100, 0, 0, 1, 1);
		}

		public void FallBeat()
		{
			base.Body.Beat(m_targer, "beat", 100, 0, 2000, 1, 1);
		}

		public void Beating()
		{
			if (m_targer != null && !base.Body.Beat(m_targer, "beat", 100, 0, 0, 1, 1))
			{
				base.Body.ChangeDirection(m_targer, 0);
				MoveToPlayer(m_targer);
			}
		}

		public void Fall()
		{
			base.Body.FallFrom(base.Body.X, base.Body.Y + 240, null, 0, 0, 12, Beating);
		}

		public SeventhNormalMaleAi()
		{


		}
	}
}
