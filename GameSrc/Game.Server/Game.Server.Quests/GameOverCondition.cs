using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests
{
	public class GameOverCondition : BaseCondition
	{
		public GameOverCondition(BaseQuest quest, QuestConditionInfo info, int value)
			: base(quest, info, value)
		{
		}

		public override void AddTrigger(GamePlayer player)
		{
			player.GameOver += player_GameOver;
		}

		public override bool IsCompleted(GamePlayer player)
		{
			return base.Value <= 0;
		}

		private int winTeam(System.Collections.Generic.List<Logic.Phy.Object.Player> players)
        {
			int winteam = -1;
			foreach (Logic.Phy.Object.Player current in players)
			{
				if (current.IsLiving)
				{
					winteam = current.Team;
					break;
				}
			}
			return winteam;
		}

		private void player_GameOver(AbstractGame game, bool isWin, int gainXp, bool isSpanArea, bool isCouple)
		{
			if (!isWin)
			{
				return;
			}
			switch (game.RoomType)
			{
			case eRoomType.Match:
				if ((m_info.Para1 == 0 || m_info.Para1 == -1) && base.Value > 0)
				{
					base.Value--;
				}
				break;
			case eRoomType.Freedom:
				if ((m_info.Para1 == 1 || m_info.Para1 == -1) && base.Value > 0)
				{
					base.Value--;
				}
				break;
			}
		}
	}
}
