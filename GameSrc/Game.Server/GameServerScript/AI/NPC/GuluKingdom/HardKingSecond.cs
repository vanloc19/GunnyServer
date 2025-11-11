using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;
using System.Drawing;

namespace GameServerScript.AI.NPC
{
	public class HardKingSecond : ABrain
	{
		private int int_0;

		private int int_1;

		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

		private int int_2;

		private int int_3;

		private static string[] string_0;

		private static string[] string_1;

		private static string[] string_2;

		private static string[] string_3;

		private static string[] string_4;

		private static string[] string_5;

		private static string[] string_6;

		private static string[] string_7;

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
			bool flag = false;
			int num = 0;
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				if (allFightPlayer.IsLiving && allFightPlayer.X > 620 && allFightPlayer.X < 1160)
				{
					int num2 = (int)base.Body.Distance(allFightPlayer.X, allFightPlayer.Y);
					if (num2 > num)
					{
						num = num2;
					}
					flag = true;
				}
			}
			if (flag)
			{
				method_3(620, 1160);
			}
			else if (int_0 == 0)
			{
				method_2();
				if (int_2 == 1)
				{
					physicalObj_0.CanPenetrate = true;
					physicalObj_1.CanPenetrate = true;
					base.Game.RemovePhysicalObj(physicalObj_0, sendToClient: true);
					base.Game.RemovePhysicalObj(physicalObj_1, sendToClient: true);
					int_2 = 0;
				}
				int_0++;
			}
			else if (int_0 == 1)
			{
				method_1();
				int_0++;
			}
			else if (int_0 == 2)
			{
				method_4();
				int_0++;
			}
			else
			{
				method_0();
				int_0 = 0;
			}
		}

		private void method_0()
		{
			base.Game.GetFrostPlayerRadom();
			List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
			List<Player> list = new List<Player>();
			foreach (Player item in allFightPlayers)
			{
				if (!item.IsFrost)
				{
					list.Add(item);
				}
			}
			((SimpleBoss)base.Body).CurrentDamagePlus = 1000f;
			if (list.Count != allFightPlayers.Count)
			{
				if (list.Count != 0)
				{
					base.Body.PlayMovie("beat1", 0, 0);
					base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "beat1", 1500, list);
				}
				else
				{
					base.Body.PlayMovie("beat1", 0, 0);
					base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "beat1", 1500, null);
				}
			}
			else
			{
				base.Body.Say("Nhỏ mà họ đã cho tôi một bài học tốt đối phương!", 1, 3300);
				base.Body.PlayMovie("renew", 3500, 0);
				base.Body.CallFuction(CreateChild, 6000);
			}
		}

		private void method_1()
		{
			int x = base.Game.Random.Next(798, 980);
			base.Body.MoveTo(x, base.Body.Y, "walk", 0, "", 4, method_5);
		}

		private void method_2()
		{
			base.Body.CurrentDamagePlus = 1.5f;
			if (int_1 == 0)
			{
				int num = base.Game.Random.Next(0, string_0.Length);
				base.Body.Say(string_0[num], 1, 13000);
				base.Body.PlayMovie("beat1", 15000, 0);
				base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 17000, null);
				int_1++;
			}
			else
			{
				int num2 = base.Game.Random.Next(0, string_0.Length);
				base.Body.Say(string_0[num2], 1, 0);
				base.Body.PlayMovie("beat1", 1000, 0);
				base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 3000, null);
			}
		}

		private void method_3(int int_4, int int_5)
		{
			int num = base.Game.Random.Next(0, string_5.Length);
			if (int_1 == 0)
			{
				base.Body.CurrentDamagePlus = 100f;
				base.Body.Say(string_5[num], 1, 13000);
				base.Body.PlayMovie("beat1", 15000, 0);
				base.Body.RangeAttacking(int_4, int_5, "cry", 17000, null);
				int_1++;
			}
			else
			{
				base.Body.CurrentDamagePlus = 100f;
				base.Body.Say(string_5[num], 1, 0);
				base.Body.PlayMovie("beat1", 2000, 0);
				base.Body.RangeAttacking(int_4, int_5, "cry", 4000, null);
			}
		}

		private void method_4()
		{
			if (int_2 == 0)
			{
				physicalObj_0 = ((PVEGame)Game).CreatePhysicalObj(Body.X - 15, 620, "wallLeft", "com.mapobject.asset.WaveAsset_01_left", "1", 1, 1, 0);
				physicalObj_1 = ((PVEGame)Game).CreatePhysicalObj(Body.X + 15, 620, "wallRight", "com.mapobject.asset.WaveAsset_01_right", "1", 1, 1, 0);
				physicalObj_0.CanPenetrate = false;
				physicalObj_1.CanPenetrate = false;
				physicalObj_0.SetRect(-165, -169, 43, 330);
				physicalObj_1.SetRect(128, -165, 41, 330);
				int_2 = 1;
			}
			int num = base.Game.Random.Next(0, string_7.Length);
			base.Body.Say(string_7[num], 1, 0);
		}

		private Point[] BrithPoint = { new Point(682, 673), new Point(1092, 673) };

		public void CreateChild()
		{
			base.Body.PlayMovie("renew", 100, 2000);
			((SimpleBoss)Body).CreateChild(int_3, BrithPoint, 8, 2, 1);
		}

		private void method_5()
		{
			int num = base.Game.Random.Next(1, 2);
			for (int i = 0; i < num; i++)
			{
				Player player = base.Game.FindRandomPlayer();
				int num2 = base.Game.Random.Next(0, string_1.Length);
				base.Body.Say(string_1[num2], 1, 0);
				if (player.X > base.Body.X)
				{
					base.Body.ChangeDirection(1, 500);
				}
				else
				{
					base.Body.ChangeDirection(-1, 500);
				}
				if (player != null && !player.IsFrost && base.Body.ShootPoint(player.X, player.Y, 1, 1000, 10000, 1, 1.5f, 2000))
				{
					base.Body.PlayMovie("beat2", 1500, 0);
				}
			}
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		public HardKingSecond()
		{
			
			int_3 = 1210;
			
		}

		static HardKingSecond()
		{
			
			string_0 = new string[3]
			{
				"Trận động đất, bản thân mình! ! <br/> bạn vui lòng Ay giúp đỡ",
				"Hạ vũ khí xuống!",
				"Xem nếu bạn có thể đủ khả năng, một số ít!！"
			};
			string_1 = new string[3]
			{
				"Cho bạn biết những gì một cú sút vết nứt!",
				"Gửi cho bạn một quả bóng - bạn phải chọn Vâng",
				"Nhóm của bạn của những người dân thường ngu dốt và thấp"
			};
			string_2 = new string[2]
			{
				"Ah ~ ~ Tại sao bạn tấn công? <br/> tôi đang làm gì?",
				"Oh ~ ~ nó thực sự đau khổ! Tại sao tôi phải chiến đấu? <br/> tôi phải chiến đấu ..."
			};
			string_3 = new string[3]
			{
				"Mathias không kiểm soát tôi!",
				"Đây là thách thức số phận của tôi!",
				"Không! !Đây không phải là ý chí của tôi ..."
			};
			string_4 = new string[3]
			{
				"Xoắn ah xoay ~ <br/>xoắn ah xoay ~ ~ ~",
				"~ Hallelujah <br/>Luyaluya ~ ~ ~",
				"Yeah Yeah Yeah, <br/> để thoải mái!"
			};
			string_5 = new string[1]
			{
				"Con rồng trong thế giới! !"
			};
			string_6 = new string[3]
			{
				"Hương vị này",
				"Hãy để bạn bình tĩnh",
				"Bạn đã giận dữ với tôi."
			};
			string_7 = new string[2]
			{
				"Chúa, cho tôi sức mạnh!",
				"Tuyệt vọng, xem tường thủy tinh của tôi!"
			};
		}
	}
}
