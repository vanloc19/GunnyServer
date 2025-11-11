using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests
{
	public class GameFihgt2v2Condition : BaseCondition
	{
		public GameFihgt2v2Condition(BaseQuest quest, QuestConditionInfo info, int value)
			: base(quest, info, value)
		{
		}

		public override void AddTrigger(GamePlayer player)
		{
			player.GameOver2v2 += player_GameOver;
		}

		public override bool IsCompleted(GamePlayer player)
		{
			return base.Value <= 0;
		}

		private void player_GameOver(bool isWin)
		{
			if (m_info.Para1 == 1)
			{
				if (isWin)
				{
					base.Value--;
				}
			}
			else
			{
				base.Value--;
			}
			if (base.Value < 0)
			{
				base.Value = 0;
			}
		}

		public override void RemoveTrigger(GamePlayer player)
		{
			player.GameOver2v2 -= player_GameOver;
		}
	}
}
