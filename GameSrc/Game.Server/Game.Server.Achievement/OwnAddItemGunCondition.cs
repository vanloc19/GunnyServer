using Game.Logic;
using Game.Server.GameObjects;

namespace Game.Server.Achievement
{
	public class OwnAddItemGunCondition : BaseUserRecord
	{
		public OwnAddItemGunCondition(GamePlayer player, int type)
			: base(player, type)
		{
			AddTrigger(player);
		}

		public override void AddTrigger(GamePlayer player)
		{
			player.AfterKillingLiving += player_AfterKillingLiving;
		}

		private void player_AfterKillingLiving(AbstractGame game, int type, int id, bool isLiving, int demage, bool isSpanArea)
		{
			int count = 0;
			count += m_player.GetItemCount(7015);
			count += m_player.GetItemCount(7016);
			count += m_player.GetItemCount(7017);
			count += m_player.GetItemCount(7018);
			count += m_player.GetItemCount(7019);
			count += m_player.GetItemCount(7020);
			count += m_player.GetItemCount(7021);
			count += m_player.GetItemCount(7022);
			count += m_player.GetItemCount(7023);
			m_player.AchievementInventory.UpdateUserAchievement(m_type, count);
		}

		public override void RemoveTrigger(GamePlayer player)
		{
			player.AfterKillingLiving -= player_AfterKillingLiving;
		}
	}
}
