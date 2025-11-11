using Bussiness;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests
{
	public class OwnConsortiaCondition : BaseCondition
	{
		public OwnConsortiaCondition(BaseQuest quest, QuestConditionInfo info, int value)
			: base(quest, info, value)
		{
		}

		public override void AddTrigger(GamePlayer player)
		{
			player.GuildChanged += player_OwnConsortia;
		}

		public override bool IsCompleted(GamePlayer player)
		{
			bool flag = false;
			int count = 0;
			using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
			{
				ConsortiaInfo consortiaSingle = bussiness.GetConsortiaSingle(player.PlayerCharacter.ConsortiaID);
				switch (m_info.Para1)
				{
				case 0:
					count = consortiaSingle.Count;
					break;
				case 1:
					count = player.PlayerCharacter.RichesOffer + player.PlayerCharacter.RichesRob;
					break;
				case 2:
					count = consortiaSingle.SmithLevel;
					break;
				case 3:
					count = consortiaSingle.ShopLevel;
					break;
				case 4:
					count = consortiaSingle.StoreLevel;
					break;
				}
				if (count >= m_info.Para2)
				{
					base.Value = 0;
					flag = true;
				}
				return flag;
			}
		}

		private void player_OwnConsortia()
		{
		}

		public override void RemoveTrigger(GamePlayer player)
		{
			player.GuildChanged -= player_OwnConsortia;
		}
	}
}
