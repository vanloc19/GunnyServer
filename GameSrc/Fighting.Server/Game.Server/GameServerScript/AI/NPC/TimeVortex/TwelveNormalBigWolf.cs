using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
    public class TwelveNormalBigWolf : ABrain
    {
        protected Living m_targer;

        protected Player m_player;

        public void Beating()
        {
			if (m_player.X > m_targer.X)
			{
				if (m_targer != null && !base.Body.Beat(m_targer, "beatA", 100, 5, 1500, 1, 1))
				{
					MoveToPlayer(m_player);
				}
			}
			else if (m_targer != null && !base.Body.Beat(m_targer, "beatA", 100, 0, 1500, 1, 1))
			{
				MoveToHelper(m_targer);
			}
        }

        public void MoveBeat()
        {
			base.Body.Beat(m_targer, "beatA", 100, 1, 1500, 1, 1);
        }

        public void MoveToHelper(Living living)
        {
			int num = (int)living.Distance(base.Body.X, base.Body.Y);
			int num2 = base.Game.Random.Next(((SimpleNpc)base.Body).NpcInfo.MoveMin, ((SimpleNpc)base.Body).NpcInfo.MoveMax);
			int delay = 0;
			if (num <= 127)
			{
				return;
			}
			num = ((num > ((SimpleNpc)base.Body).NpcInfo.MoveMax) ? num2 : (num - base.Body.MaxBeatDis));
			if (living.Y < 420 && living.X < 210)
			{
				if (base.Body.Y > 420)
				{
					if (base.Body.X - num < 50)
					{
						delay = Game.GetDelayDistance(Body.X, 25, 4) + 500;
						base.Body.MoveTo(25, base.Body.Y, "walk", (base.Body as SimpleNpc).NpcInfo.speed, "", 1200, MoveBeat, delay - 800);
					}
					else
					{
						delay = Game.GetDelayDistance(Body.X, Body.X - num, 4) + 500;
						base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", (base.Body as SimpleNpc).NpcInfo.speed, "", 1200, MoveBeat, delay - 800);
					}
				}
				else if (living.X > base.Body.X)
				{
					delay = Game.GetDelayDistance(Body.X, Body.X + num, 4) + 500;
					base.Body.MoveTo(base.Body.X + num, base.Body.Y, "walk", (base.Body as SimpleNpc).NpcInfo.speed, "", 1200, MoveBeat, delay - 800);
				}
				else
				{
					delay = Game.GetDelayDistance(Body.X, Body.X - num, 4) + 500;
					base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", (base.Body as SimpleNpc).NpcInfo.speed, "", 1200, MoveBeat, delay - 800);
				}
			}
			else if (base.Body.Y >= 420)
			{
				if (living.X > base.Body.X)
				{
					delay = Game.GetDelayDistance(Body.X, Body.X + num, 4) + 500;
					base.Body.MoveTo(base.Body.X + num, base.Body.Y, "walk", (base.Body as SimpleNpc).NpcInfo.speed, "", 1200, MoveBeat, delay - 800);
				}
				else
				{
					delay = Game.GetDelayDistance(Body.X, Body.X - num, 4) + 500;
					base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", (base.Body as SimpleNpc).NpcInfo.speed, "", 1200, MoveBeat, delay - 800);
				}
			}
			else if (base.Body.X + num > 200)
			{
				delay = Game.GetDelayDistance(Body.X, 200, 4) + 500;
				base.Body.MoveTo(200, base.Body.Y, "walk", (base.Body as SimpleNpc).NpcInfo.speed, "", 1200, MoveBeat, delay - 800);
			}
        }

        public void MoveToPlayer(Player player)
        {
			int num = (int)player.Distance(base.Body.X, base.Body.Y);
			int num2 = base.Game.Random.Next(((SimpleNpc)base.Body).NpcInfo.MoveMin, ((SimpleNpc)base.Body).NpcInfo.MoveMax);
			int delay = 0;
			if (num <= 97)
			{
				return;
			}
			num = ((num > ((SimpleNpc)base.Body).NpcInfo.MoveMax) ? num2 : (num - 90));
			if (player.Y < 420 && player.X < 210)
			{
				if (base.Body.Y > 420)
				{
					if (base.Body.X - num < 50)
					{
						delay = Game.GetDelayDistance(Body.X, 25, 4) + 500;
						base.Body.MoveTo(25, base.Body.Y, "walk", (base.Body as SimpleNpc).NpcInfo.speed, "", 1200, MoveBeat, delay - 800);
					}
					else
					{
						delay = Game.GetDelayDistance(Body.X, Body.X - num, 4) + 500;
						base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", (base.Body as SimpleNpc).NpcInfo.speed, "", 1200, MoveBeat, delay - 800);
					}
				}
				else if (player.X > base.Body.X)
				{
					delay = Game.GetDelayDistance(Body.X, Body.X + num, 4) + 500;
					base.Body.MoveTo(base.Body.X + num, base.Body.Y, "walk", (base.Body as SimpleNpc).NpcInfo.speed, "", 1200, MoveBeat, delay - 800);
				}
				else
				{
					delay = Game.GetDelayDistance(Body.X, Body.X - num, 4) + 500;
					base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", (base.Body as SimpleNpc).NpcInfo.speed, "", 1200, MoveBeat, delay - 800);
				}
			}
			else if (base.Body.Y >= 420)
			{
				if (player.X > base.Body.X)
				{
					delay = Game.GetDelayDistance(Body.X, Body.X + num, 4) + 500;
					base.Body.MoveTo(base.Body.X + num, base.Body.Y, "walk", (base.Body as SimpleNpc).NpcInfo.speed, "", 1200, MoveBeat, delay - 800);
				}
				else
				{
					delay = Game.GetDelayDistance(Body.X, Body.X - num, 4) + 500;
					base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", (base.Body as SimpleNpc).NpcInfo.speed, "", 1200, MoveBeat, delay - 800);
				}
			}
			else if (base.Body.X + num > 200)
			{
				delay = Game.GetDelayDistance(Body.X, 200, 4) + 500;
				base.Body.MoveTo(200, base.Body.Y, "walk", (base.Body as SimpleNpc).NpcInfo.speed, "", 1200, MoveBeat, delay - 800);
			}
		}


		public override void OnBeginNewTurn()
        {
			base.OnBeginNewTurn();
			m_body.CurrentDamagePlus = 1f;
			m_body.CurrentShootMinus = 1f;
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
			base.OnStartAttacking();
			m_targer = base.Game.FindNearestHelper(base.Body.X, base.Body.Y);
			m_player = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
			Beating();
        }

        public override void OnStopAttacking()
        {
			base.OnStopAttacking();
        }
    }
}
