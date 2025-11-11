using System.Collections.Generic;
using System.Drawing;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class FiveNormalThirdBoss : ABrain
	{
		private int int_0;

		protected Player m_targer;

		private List<PhysicalObj> list_0;

		private PhysicalObj physicalObj_0;

		private SimpleNpc simpleNpc_0;

		private SimpleNpc simpleNpc_1;

		private SimpleNpc simpleNpc_2;

		private int int_1;

		private int int_2;

		private int int_3;

		private int int_4;

		private int int_5;

		private List<Point> list_1;

		private string[] string_0;

		private string[] string_1;

		private string[] string_2;

		private string[] string_3;

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
				base.Game.RemovePhysicalObj(item, true);
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
			switch (int_0)
			{
				case 1:
					method_4();
					break;
				case 2:
					if (simpleNpc_1 != null && simpleNpc_1.IsLiving)
					{
						method_4();
					}
					else
					{
						method_9();
					}
					break;
				case 3:
					method_0();
					break;
				case 4:
					if (simpleNpc_2 != null && simpleNpc_2.IsLiving)
					{
						method_4();
					}
					else
					{
						method_7();
					}
					break;
				case 5:
					if (simpleNpc_0 != null && simpleNpc_0.IsLiving)
					{
						method_0();
					}
					else
					{
						method_5();
					}
					int_0 = 0;
					break;
			}
		}

		private void method_0()
		{
			base.Body.ChangeDirection(-1, 500);
			base.Body.MoveTo(1000, 641, "fly", 1000, method_1, 10);
		}

		private void method_1()
		{
			base.Body.CurrentDamagePlus = 2f;
			((SimpleBoss)base.Body).RandomSay(string_1, 0, 1000, 0);
			base.Body.PlayMovie("beatA", 1200, 0);
			base.Body.CallFuction(method_2, 2500);
			base.Body.RangeAttacking(base.Body.X - 10000, base.Body.X + 10000, "cry", 4500, directDamage: true);
			base.Body.PlayMovie("beatC", 4000, 2000);
		}

		private void method_2()
		{
			physicalObj_0 = ((PVEGame)base.Game).CreateLayerTop(500, 300, "", "asset.game.4.heip", "", 1, 1);
			base.Body.CallFuction(method_3, 2400);
		}

		private void method_3()
		{
			if (physicalObj_0 != null)
			{
				base.Game.RemovePhysicalObj(physicalObj_0, true);
			}
		}

		private void method_4()
		{
			m_targer = base.Game.FindRandomPlayer();
			if (m_targer != null)
			{
				((SimpleBoss)base.Body).RandomSay(string_0, 0, 1000, 0);
				base.Body.MoveTo(m_targer.X, m_targer.Y - 100, "fly", 2000, method_13, 10);
			}
		}

		private void method_5()
		{
			((SimpleBoss)base.Body).RandomSay(string_2, 0, 1000, 0);
			base.Body.PlayMovie("beatD", 1200, 0);
			base.Body.CallFuction(method_6, 3800);
		}

		private void method_6()
		{
			int x = base.Game.Random.Next(440, 1570);
			LivingConfig livingConfig = ((PVEGame)base.Game).BaseLivingConfig();
			livingConfig.DamageForzen = true;
			livingConfig.CanTakeDamage = false;
			livingConfig.IsFly = true;
			simpleNpc_0 = ((SimpleBoss)base.Body).CreateChild(int_1, x, 580, 1, false, livingConfig);
			simpleNpc_0.Properties2 = int_5;
		}

		private void method_7()
		{
			m_targer = base.Game.FindRandomPlayer();
			if (m_targer != null)
			{
				base.Body.ChangeDirection(m_targer, 500);
				int num = base.Game.Random.Next(string_3.Length);
				base.Body.Say(string.Format(string_3[num], m_targer.PlayerDetail.PlayerCharacter.NickName), 0, 1000);
				base.Body.PlayMovie("beatD", 3000, 0);
				base.Body.CallFuction(method_8, 5000);
			}
		}

		private void method_8()
		{
			LivingConfig livingConfig = ((PVEGame)base.Game).BaseLivingConfig();
			livingConfig.IsFly = true;
			simpleNpc_2 = ((SimpleBoss)base.Body).CreateChild(int_3, 114, 453, 1, false, livingConfig);
			simpleNpc_2.Properties1 = m_targer.Id;
			simpleNpc_2.Properties2 = new Point(m_targer.X, m_targer.Y);
			int highDelayTurn = base.Game.GetHighDelayTurn();
			((PVEGame)base.Game).SendObjectFocus(m_targer, 1, 1500, 0);
			base.Body.CallFuction(method_11, 2500);
			base.Body.CallFuction(method_12, 3500);
			m_targer.BoltMove(simpleNpc_2.X, simpleNpc_2.Y, 3900);
			((PVEGame)base.Game).SendObjectFocus(simpleNpc_2, 1, 4000, 0);
			simpleNpc_2.PlayMovie("in", 5000, 4000);
			((PVEGame)base.Game).PveGameDelay = highDelayTurn + 1;
		}

		private void method_9()
		{
			m_targer = base.Game.FindRandomPlayer();
			if (m_targer != null)
			{
				base.Body.ChangeDirection(m_targer, 500);
				base.Body.Say("Bắt lại.. Ta cần bắt <span class=\"red\">" + m_targer.PlayerDetail.PlayerCharacter.NickName + "</span> nhốt lại.", 0, 1000);
				base.Body.PlayMovie("beatD", 3000, 0);
				base.Body.CallFuction(method_10, 5000);
			}
		}

		private void method_10()
		{
			int index = base.Game.Random.Next(list_1.Count);
			simpleNpc_1 = ((SimpleBoss)base.Body).CreateChild(int_2, list_1[index].X, list_1[index].Y, true, ((PVEGame)base.Game).BaseLivingConfig());
			simpleNpc_1.Properties1 = m_targer.Id;
			int highDelayTurn = base.Game.GetHighDelayTurn();
			((PVEGame)base.Game).SendObjectFocus(m_targer, 1, 4000, 0);
			base.Body.CallFuction(method_11, 5000);
			base.Body.CallFuction(method_12, 6000);
			m_targer.BoltMove(simpleNpc_1.X, simpleNpc_1.Y, 6100);
			((PVEGame)base.Game).SendObjectFocus(simpleNpc_1, 1, 6800, 0);
			simpleNpc_1.PlayMovie("AtoB", 7500, 0);
			simpleNpc_1.PlayMovie("beatA", 10000, 2000);
			base.Body.BeatDirect(m_targer, "", 11000, 1, 1);
			((PVEGame)base.Game).PveGameDelay = highDelayTurn + 1;
		}

		private void method_11()
		{
			list_0.Add(((PVEGame)base.Game).Createlayer(m_targer.X, m_targer.Y, "", "asset.game.4.lanhuo", "", 1, 1));
		}

		private void method_12()
		{
			m_targer.SetVisible(false);
			m_targer.BlockTurn = true;
		}

		private void method_13()
		{
			base.Body.Beat(m_targer, "beatE", 100, 1, 500, 1, 1);
			base.Body.CallFuction(method_14, 3500);
		}

		private void method_14()
		{
			int x = base.Game.Random.Next(376, 1643);
			int y = base.Game.Random.Next(112, 593);
			base.Body.MoveTo(x, y, "fly", 500, 10);
		}

		private void method_15()
		{
			base.Body.Say("Thôi ta éo đùa nữa.", 0, 500);
			base.Body.PlayMovie("out", 500, 0);
			base.Body.CallFuction(method_16, 3500);
		}

		private void method_16()
		{
			SimpleNpc simpleNpc = ((PVEGame)base.Game).CreateNpc(int_4, 0, 0, 1, 1, "standC", ((PVEGame)base.Game).BaseLivingConfig());
			simpleNpc.PlayMovie("cool", 1000, 0);
			simpleNpc.Say("Thôi hãy rời khỏi đây mau. Chúa rồng đã sống lại rồi.", 0, 4000, 2000);
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
			bool isLiving = base.Body.IsLiving;
		}

		public FiveNormalThirdBoss()
		{
			list_0 = new List<PhysicalObj>();
			int_1 = 5122;
			int_2 = 5123;
			int_3 = 5124;
			int_4 = 5104;
			int_5 = 1;
			list_1 = new List<Point>
			{
				new Point(1518, 699),
				new Point(500, 699)
			};
			string_0 = new string[3]
			{
				"Phạt.. phạt...",
				"Xem lưỡi hái của ta này.",
				"Đủ trình để tiếp chiêu?"
			};
			string_1 = new string[3]
			{
				"Cú này đau lắm đấy!",
				"Xem tuyệt chiêu của ta.",
				"Tra tấn là sở thích của ta."
			};
			string_2 = new string[3]
			{
				"Ta chuẩn bị cho các ngươi món quà khá hot đấy.",
				"Món quà nhỏ dành cho các ngươi này.",
				"Mỏ hàn đâu ra dí ass bọn nó.."
			};
			string_3 = new string[3]
			{
				"<span class=\"red\">{0}</span> hãy xem cái này nhé.",
				"<span class=\"red\">{0}</span> thử cái này xem.",
				"Hãy xem <span class=\"red\">{0}</span> bị xử kìa."
			};
		}
	}
}
