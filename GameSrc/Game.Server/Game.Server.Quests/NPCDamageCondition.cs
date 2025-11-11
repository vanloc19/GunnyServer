using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests
{
	public class NPCDamageCondition : BaseCondition
	{
		public NPCDamageCondition(BaseQuest quest, QuestConditionInfo info, int value)
			: base(quest, info, value)
		{
		}

		public override void AddTrigger(GamePlayer player)
		{
			player.AfterDamageBoss += player_NPCDamage;
		}

		public override bool IsCompleted(GamePlayer player)
		{
			return base.Value >= m_info.Para2;
		}

		private void player_NPCDamage(AbstractGame game, NpcInfo npc, int damage)
		{
			if (npc.ID == m_info.Para1)
			{
				base.Value += damage;
			}
		}

		public override void RemoveTrigger(GamePlayer player)
		{
			player.AfterDamageBoss -= player_NPCDamage;
		}
	}
}
