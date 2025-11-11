using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Achievement
{
	public class ChangeColorsCondition : BaseUserRecord
	{
		public ChangeColorsCondition(GamePlayer player, int type)
			: base(player, type)
		{
			AddTrigger(player);
		}

		public override void AddTrigger(GamePlayer player)
		{
			player.PlayerPropertyChanged += player_PlayerPropertyChanged;
		}

		private void player_PlayerPropertyChanged(PlayerInfo character)
		{
			string[] color = character.Colors.Split(',');
			int count = 0;
			for (int i = 0; i < color.Length; i++)
			{
				if (color[i].ToString() != "" && color[i].ToString() != "|")
				{
					count++;
				}
			}
			m_player.AchievementInventory.UpdateUserAchievement(m_type, count, 1);
		}

		public override void RemoveTrigger(GamePlayer player)
		{
			player.PlayerPropertyChanged -= player_PlayerPropertyChanged;
		}
	}
}
