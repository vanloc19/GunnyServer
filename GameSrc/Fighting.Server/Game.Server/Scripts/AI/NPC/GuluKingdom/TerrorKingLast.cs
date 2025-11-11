using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
	public class TerrorKingLast : ABrain
	{
		private int int_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private int int_4;

		public List<SimpleNpc> orchins;

		private static string[] string_0;

		private static string[] string_1;

		private static string[] string_2;

		private static string[] string_3;

		private static string[] string_4;

		private static string[] string_5;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			base.Body.CurrentDamagePlus = 1f;
			base.Body.CurrentShootMinus = 1f;
		}

		public override void OnCreated()
		{
			base.OnCreated();
		}

		public override void OnStartAttacking()
		{
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
				KillAttack(620, 1160);
			}
			else if (!flag)
			{
				if (int_0 == 1)
				{
					base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
					HalfAttack();
				}
				else if (int_0 == 2)
				{
					base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
					Summon();
				}
				else if (int_0 == 3)
				{
					base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
					Seal();
				}
				else if (int_0 == 4)
				{
					base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
					Angger();
				}
				else
				{
					base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
					GoOnAngger();
					int_0 = 0;
				}
				int_0++;
			}
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		public void HalfAttack()
		{
			int num = base.Game.Random.Next(0, string_4.Length);
			base.Body.Say(string_0[num], 1, 500);
			base.Body.PlayMovie("beatB", 2500, 0);
			if (base.Body.Direction == 1)
			{
				base.Body.RangeAttacking(base.Body.X, base.Body.X + 1000, "cry", 3300, null);
			}
			else
			{
				base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X, "cry", 3300, null);
			}
		}

		public void Summon()
		{
			int num = base.Game.Random.Next(0, string_1.Length);
			base.Body.Say(string_1[num], 1, 0);
			base.Body.PlayMovie("beatA", 100, 0);
			base.Body.CallFuction(CreateChild, 2500);
		}

		public void Seal()
		{
			int num = base.Game.Random.Next(0, string_4.Length);
			((SimpleBoss)base.Body).Say(string_4[num], 1, 0);
			Player player = base.Game.FindRandomPlayer();
			base.Body.PlayMovie("mantra", 2000, 2000);
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				allLivingPlayer.AddEffect(new LockDirectionEffect(2), 0);
			}
			base.Body.Seal(player, 1, 3000);
		}

		public void Angger()
		{
			int num = base.Game.Random.Next(0, string_2.Length);
			base.Body.Say(string_2[num], 1, 0);
			base.Body.State = 1;
			int_3 += 100;
			base.Body.PlayMovie("angry", 1000, 0);
			((SimpleBoss)base.Body).SetDander(int_3);
			if (base.Body.Direction == -1)
			{
				((SimpleBoss)base.Body).SetRelateDemagemRect(8, -252, 74, 50);
			}
			else
			{
				((SimpleBoss)base.Body).SetRelateDemagemRect(-82, -252, 74, 50);
			}
		}

		public void GoOnAngger()
		{
			if (base.Body.State == 1)
			{
				base.Body.CurrentDamagePlus = 1000f;
				base.Body.PlayMovie("beatC", 3500, 0);
				base.Body.RangeAttacking(base.Body.X - 10000, base.Body.X + 10000, "cry", 5600, null);
				base.Body.Die(5600);
			}
			else
			{
				((SimpleBoss)base.Body).SetRelateDemagemRect(-41, -187, 83, 140);
				base.Body.PlayMovie("mantra", 0, 2000);
				foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
				{
					allLivingPlayer.AddEffect(new ContinueReduceBloodEffect(2, 600, allLivingPlayer), 0);
				}
			}
		}

		public void KillAttack(int fx, int mx)
		{
			base.Body.CurrentDamagePlus = 100f;
			int num = base.Game.Random.Next(0, string_3.Length);
			((SimpleBoss)base.Body).Say(string_3[num], 1, 500);
			base.Body.PlayMovie("beatB", 2500, 0);
			base.Body.RangeAttacking(fx, mx, "cry", 3300, null);
		}

		public void CreateChild()
		{
			((SimpleBoss)base.Body).CreateChild(int_4, 680, 680, -1, 405, -1);
		}

		public TerrorKingLast()
		{
			
			int_0 = 1;
			int_1 = 1;
			int_4 = 1304;
			orchins = new List<SimpleNpc>();
			
		}

		static TerrorKingLast()
		{
			
			string_0 = new string[4]
			{
				"Nghiên cứu kỹ năng của tôi!",
				"Di chuyển mát mẻ!<br/>Bạn muốn tìm hiểu không?",
				"Chụi không nỗi!",
				"Bạn sẽ trả giá cho việc này! "
			};
			string_1 = new string[1]
			{
				"Nào, <br/>cho thử sức mạnh của lựu đạn!"
			};
			string_2 = new string[1]
			{
				"Bạn buộc tôi để lừa!"
			};
			string_3 = new string[1]
			{
				"Bạn đến chết?"
			};
			string_4 = new string[1]
			{
				"Chầu Diêm Vương!"
			};
			string_5 = new string[2]
			{
				"Địa ngục là điểm đến duy nhất của bạn!",
				"Quá dễ bị tổn thương."
			};
		}
	}
}
