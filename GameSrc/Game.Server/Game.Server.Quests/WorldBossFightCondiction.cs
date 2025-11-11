using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests
{
	public class WorldBossFightCondiction : BaseCondition
	{
		public WorldBossFightCondiction(BaseQuest quest, QuestConditionInfo info, int value)
			: base(quest, info, value)
		{
		}

		public override void AddTrigger(GamePlayer player)
		{
			player.WorldBossDamageEvent += player_WorldBossDamage;
		}

		public override bool IsCompleted(GamePlayer player)
		{
			return base.Value >= m_info.Para2;
		}

		private void player_WorldBossDamage(GamePlayer player)
		{
			base.Value = player.PlayerCharacter.damageScores;
		}

		public override void RemoveTrigger(GamePlayer player)
		{
			player.WorldBossDamageEvent -= player_WorldBossDamage;
		}
	}
}
