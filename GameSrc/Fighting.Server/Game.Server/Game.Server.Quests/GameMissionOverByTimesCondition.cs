using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests
{
	public class GameMissionOverByTimesCondition : BaseCondition
	{
		public GameMissionOverByTimesCondition(BaseQuest quest, QuestConditionInfo info, int value)
			: base(quest, info, value)
		{
		}

		public override void AddTrigger(GamePlayer player)
		{
			player.MissionFullOver += player_MissionOver;
		}

		public override bool IsCompleted(GamePlayer player)
		{
			return base.Value <= 0;
		}

		private void player_MissionOver(AbstractGame game, int missionId, bool isWin, int turnNum)
		{
			if (!isWin)
            {
				return;
            }
			if ((missionId == m_info.Para1 || m_info.Para1 == -1) && base.Value > 0)
			{
				base.Value--;
			}
		}

		public override void RemoveTrigger(GamePlayer player)
		{
			player.MissionFullOver -= player_MissionOver;
		}
	}
}
