using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests
{
	public class UpLevelVipCondition : BaseCondition
	{
		public UpLevelVipCondition(BaseQuest quest, QuestConditionInfo info, int value)
			: base(quest, info, value)
		{
		}

		public override void AddTrigger(GamePlayer player)
		{
			player.Event_0 += player_PlayerVIPUpgrade;
		}

		public override bool IsCompleted(GamePlayer player)
		{
			if (player.PlayerCharacter.VIPLevel >= m_info.Para2)
			{
				base.Value = 0;
				return true;
			}
			return false;
		}

		private void player_PlayerVIPUpgrade(int level, int exp)
		{
			base.Value = m_info.Para2 - level;
		}

		public override void RemoveTrigger(GamePlayer player)
		{
			player.Event_0 -= player_PlayerVIPUpgrade;
		}
	}
}
