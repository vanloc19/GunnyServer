using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class EighthKato : ABrain
	{
		private Player player_0;

		private int int_0;

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
			base.OnStartAttacking();
			player_0 = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
			int_0 = (int)player_0.Distance(base.Body.X, base.Body.Y);
			if (int_0 < 100)
			{
				base.Body.PlayMovie("beatA", 1000, 4000);
				base.Body.BeatDirect(player_0, "", 3000, 1, 1);
			}
			else
			{
				MoveToPlayer(player_0);
			}
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		public void MoveToPlayer(Player player)
		{
			int num = base.Game.Random.Next(((SimpleNpc)base.Body).NpcInfo.MoveMin, ((SimpleNpc)base.Body).NpcInfo.MoveMax);
			bool flag = false;
			if (!((player.X <= base.Body.X) ? base.Body.MoveTo((base.Body.X - num > player.X) ? (base.Body.X - num) : (player.X + 50), base.Body.Y, "walk", 2000, "", 4, Beat) : base.Body.MoveTo((base.Body.X + num < player.X) ? (base.Body.X + num) : (player.X - 50), base.Body.Y, "walk", 2000, "", 4, Beat)))
			{
				base.Body.CallFuction(Beat, 1000);
			}
		}

		public void Beat()
		{
			int_0 = (int)player_0.Distance(base.Body.X, base.Body.Y);
			if (int_0 < 100)
			{
				base.Body.PlayMovie("beatA", 1000, 4000);
				base.Body.BeatDirect(player_0, "", 3000, 1, 1);
			}
		}
	}
}
