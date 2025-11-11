using System;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class ThirdNormalShortNpcThird : ABrain
	{
		protected Player m_targer;

		private static Random random_0;

		private static string[] string_0;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			m_body.CurrentDamagePlus = 1f;
			m_body.CurrentShootMinus = 1f;
			if (m_body.IsSay)
			{
				string oneChat = GetOneChat();
				int delay = base.Game.Random.Next(0, 5000);
				m_body.Say(oneChat, 0, delay);
			}
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
					if (base.Body.X - num < 50)
					{
						base.Body.MoveTo(25, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, Jump);
					}
					else
					{
						base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, MoveBeat);
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
			base.Body.Beat(m_targer, "beatA", 100, 0, 0, 1, 1);
		}

		public void FallBeat()
		{
			base.Body.Beat(m_targer, "beatA", 100, 0, 2000, 1, 1);
		}

		public void Jump()
		{
			base.Body.Direction = 1;
			base.Body.JumpTo(base.Body.X, base.Body.Y - 240, "Jump", 0, 2, 3, Beating);
		}

		public void Beating()
		{
			if (m_targer != null && !base.Body.Beat(m_targer, "beatA", 100, 0, 0, 1, 1))
			{
				MoveToPlayer(m_targer);
			}
		}

		public void Fall()
		{
			base.Body.FallFrom(base.Body.X, base.Body.Y + 240, null, 0, 0, 12, Beating);
		}

		public static string GetOneChat()
		{
			int num = random_0.Next(0, string_0.Length);
			return string_0[num];
		}

		static ThirdNormalShortNpcThird()
		{
			random_0 = new Random();
			string_0 = new string[6]
			{
				"Để tôn vinh! Để giành chiến thắng! !",
				"Tổ chức cướp vũ khí của họ, không run sợ!",
				"Kẻ thù ở phía trước, sẵn sàng chiến đấu!",
				"Sức mạnh bộ tộc là bất diệt!",
				"Nhanh chóng để tiêu diệt kẻ thù!",
				"Bộ tộc của chúng ta là vô địch"
			};
		}
	}
}
