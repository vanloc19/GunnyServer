using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests
{
	public class GameFight3VS3Condiction : BaseCondition
	{
		public GameFight3VS3Condiction(BaseQuest quest, QuestConditionInfo info, int value)
			: base(quest, info, value)
		{
		}

		public override void AddTrigger(GamePlayer player)
		{
			player.GameOver3v3 += player_GameOver;
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
			player.GameOver3v3 -= player_GameOver;
		}
	}
}
