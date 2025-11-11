using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
	public class SixTerrorThirdNpc : ABrain
	{
		private int int_0;

		private bool bool_0;

		private Player player_0;

		private Dictionary<int, int> dictionary_0;

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
			base.Body.MaxBeatDis = 200;
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			int_0++;
			if (bool_0)
			{
				bool_0 = false;
				int num = dictionary_0[player_0.PlayerDetail.PlayerCharacter.ID];
				if (num > 2)
				{
					base.Body.Say("<span class='red'>" + player_0.PlayerDetail.PlayerCharacter.NickName + "</span> nhắc nhiều vẫn lì. Ra khỏi sân ngay!!", 0, 1000);
					base.Body.PlayMovie("beat", 1000, 5000);
					player_0.SyncAtTime = true;
					player_0.Die();
				}
				else if (num == 2)
				{
					base.Body.Say("<span class='red'>" + player_0.PlayerDetail.PlayerCharacter.NickName + "</span> đánh trọng tài lần nữa ta sẽ đuổi khỏi sân.", 0, 1000, 4000);
				}
				else
				{
					base.Body.Say("<span class='red'>" + player_0.PlayerDetail.PlayerCharacter.NickName + "</span> đánh trọng tài là vi phạm quy tắc.", 0, 1000, 4000);
				}
			}
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

		public override void OnAfterTakeDamage(Living source)
		{
			base.OnAfterTakeDamage(source);
			if (source is Player && !bool_0)
			{
				player_0 = (source as Player);
				int iD = (source as Player).PlayerDetail.PlayerCharacter.ID;
				if (dictionary_0.ContainsKey(iD))
				{
					Dictionary<int, int> dictionary = dictionary_0;
					int key = iD;
					dictionary[key]++;
				}
				else
				{
					dictionary_0.Add(iD, 1);
				}
				(base.Game as PVEGame).SendLivingActionMapping(base.Body, "stand", "stand");
				(base.Body as SimpleBoss).Delay = base.Game.GetLowDelayTurn() - 1;
				bool_0 = true;
				base.Body.Config.CompleteStep = false;
			}
		}

		public SixTerrorThirdNpc()
		{
			
			dictionary_0 = new Dictionary<int, int>();
			
		}
	}
}
