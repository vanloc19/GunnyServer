using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
	public class SeventhSimpleNpc : ABrain
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
			base.Body.Beat(m_targer, "beat", 100, 0, 500, 1, 1);
		}

		public void FallBeat()
		{
			base.Body.Beat(m_targer, "beat", 100, 0, 2000, 1, 1);
		}

		public void Jump()
		{
			base.Body.Direction = 1;
			base.Body.JumpTo(base.Body.X, base.Body.Y - 240, "Jump", 0, 2, 3, Beating);
		}

		public void Beating()
		{
			if (m_targer != null && !base.Body.Beat(m_targer, "beat", 100, 0, 500, 1, 1))
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

		public static void LivingSay(List<Living> livings)
		{
			if (livings == null || livings.Count == 0)
			{
				return;
			}
			int num = 0;
			int count = livings.Count;
			foreach (Living living in livings)
			{
				living.IsSay = false;
			}
			num = ((count <= 5) ? random_0.Next(0, 2) : ((count <= 5 || count > 10) ? random_0.Next(1, 4) : random_0.Next(1, 3)));
			if (num <= 0)
			{
				return;
			}
			int num2 = 0;
			while (num2 < num)
			{
				int index = random_0.Next(0, count);
				if (!livings[index].IsSay)
				{
					livings[index].IsSay = true;
					int delay = random_0.Next(0, 5000);
					livings[index].Say(GetOneChat(), 0, delay);
					num2++;
				}
			}
		}

		public SeventhSimpleNpc()
		{


		}

		static SeventhSimpleNpc()
		{

			random_0 = new Random();
			string_0 = new string[8]
			{
				"Đừng để họ vượt qua !",
				"Cướp vũ khí của chúng mau lên",
				"Hạ hết vũ khí xuống!",
				"Tiêu diệt kẻ thù!",
				"Còn ngoan cố chúng tôi sẻ không tha",
				"Đối với chiến thắng đệm Boo! Brothers phí!",
				"Nhanh chóng tiêu diệt kẻ thù! ",
				"Với sức mạnh số 1! "
			};
		}
	}
}
