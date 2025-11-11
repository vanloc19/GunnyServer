using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class TwelveSimpleSmallWolf : ABrain
	{
		protected Living m_targer;

		protected Player m_player;

		public void Beating()
		{
			if (m_player.X > m_targer.X)
			{
				if (this.m_targer != null && !base.Body.Beat(this.m_targer, "beatA", 100, 0, 0, 1, 1))
				{
					this.MoveToPlayer(this.m_player);
				}
			}
			else
			{
				if (this.m_targer != null && !base.Body.Beat(this.m_targer, "beatA", 100, 0, 0, 1, 1))
				{
					this.MoveToHelper(this.m_targer);
				}
			}
		}

		public void MoveBeat()
		{
			base.Body.Beat(this.m_targer, "beatA", 100, 0, 0, 1, 1);
		}

		public void MoveToHelper(Living living)
		{
			int num = (int)living.Distance(base.Body.X, base.Body.Y);
			int num1 = base.Game.Random.Next(((SimpleNpc)base.Body).NpcInfo.MoveMin, ((SimpleNpc)base.Body).NpcInfo.MoveMax);
			if (num > 127)
			{
				if (num <= ((SimpleNpc)base.Body).NpcInfo.MoveMax)
				{
					num -= Body.MaxBeatDis;
				}
				else
				{
					num = num1;
				}
				if (living.Y < 420 && living.X < 210)
				{
					if (base.Body.Y > 420)
					{
						if (base.Body.X - num < 50)
						{
							base.Body.MoveTo(25, base.Body.Y, "walk", 1200, "", (base.Body as SimpleNpc).NpcInfo.speed, new LivingCallBack(this.MoveBeat));
							return;
						}
						base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", 1200, "", (base.Body as SimpleNpc).NpcInfo.speed, new LivingCallBack(this.MoveBeat));
						return;
					}
					if (living.X > base.Body.X)
					{
						base.Body.MoveTo(base.Body.X + num, base.Body.Y, "walk", 1200, "", (base.Body as SimpleNpc).NpcInfo.speed, new LivingCallBack(this.MoveBeat));
						return;
					}
					base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", 1200, "", (base.Body as SimpleNpc).NpcInfo.speed, new LivingCallBack(this.MoveBeat));
					return;
				}
				if (base.Body.Y >= 420)
				{
					if (living.X > base.Body.X)
					{
						base.Body.MoveTo(base.Body.X + num, base.Body.Y, "walk", 1200, "", (base.Body as SimpleNpc).NpcInfo.speed, new LivingCallBack(this.MoveBeat));
						return;
					}
					base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", 1200, "", (base.Body as SimpleNpc).NpcInfo.speed, new LivingCallBack(this.MoveBeat));
				}
				else if (base.Body.X + num > 200)
				{
					base.Body.MoveTo(200, base.Body.Y, "walk", 1200, "", (base.Body as SimpleNpc).NpcInfo.speed, new LivingCallBack(this.MoveBeat));
					return;
				}
			}
		}
		public void MoveToPlayer(Player player)
		{
			int num = (int)player.Distance(base.Body.X, base.Body.Y);
			int num1 = base.Game.Random.Next(((SimpleNpc)base.Body).NpcInfo.MoveMin, ((SimpleNpc)base.Body).NpcInfo.MoveMax);
			if (num > 97)
			{
				if (num <= ((SimpleNpc)base.Body).NpcInfo.MoveMax)
				{
					num -= 90;
				}
				else
				{
					num = num1;
				}
				if (player.Y < 420 && player.X < 210)
				{
					if (base.Body.Y > 420)
					{
						if (base.Body.X - num < 50)
						{
							base.Body.MoveTo(25, base.Body.Y, "walk", 1200, "", (base.Body as SimpleNpc).NpcInfo.speed, new LivingCallBack(this.MoveBeat));
							return;
						}
						base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", 1200, "", (base.Body as SimpleNpc).NpcInfo.speed, new LivingCallBack(this.MoveBeat));
						return;
					}
					if (player.X > base.Body.X)
					{
						base.Body.MoveTo(base.Body.X + num, base.Body.Y, "walk", 1200, "", (base.Body as SimpleNpc).NpcInfo.speed, new LivingCallBack(this.MoveBeat));
						return;
					}
					base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", 1200, "", (base.Body as SimpleNpc).NpcInfo.speed, new LivingCallBack(this.MoveBeat));
					return;
				}
				if (base.Body.Y >= 420)
				{
					if (player.X > base.Body.X)
					{
						base.Body.MoveTo(base.Body.X + num, base.Body.Y, "walk", 1200, "", (base.Body as SimpleNpc).NpcInfo.speed, new LivingCallBack(this.MoveBeat));
						return;
					}
					base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", 1200, "", (base.Body as SimpleNpc).NpcInfo.speed, new LivingCallBack(this.MoveBeat));
				}
				else if (base.Body.X + num > 200)
				{
					base.Body.MoveTo(200, base.Body.Y, "walk", 1200, "", (base.Body as SimpleNpc).NpcInfo.speed, new LivingCallBack(this.MoveBeat));
					return;
				}
			}
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			this.m_body.CurrentDamagePlus = 1f;
			this.m_body.CurrentShootMinus = 1f;
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
			this.m_targer = base.Game.FindNearestHelper(base.Body.X, base.Body.Y);
			this.m_player = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
			this.Beating();
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}
	}
}