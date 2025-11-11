using Game.Logic;
using Game.Server.ConsortiaTask.Data;
using SqlDataProvider.Data;

namespace Game.Server.ConsortiaTask.Condition
{
	public class GameFight2v2Condition : BaseConsortiaTaskCondition
	{
		public GameFight2v2Condition(ConsortiaTaskUserDataInfo player, BaseConsortiaTask quest, ConsortiaTaskInfo info, int value)
			: base(player, quest, info, value)
		{
		}

		public override void AddTrigger(ConsortiaTaskUserDataInfo player)
		{
			player.Player.GameOverCountTeam += method_0;
		}

		public override void RemoveTrigger(ConsortiaTaskUserDataInfo player)
		{
			player.Player.GameOverCountTeam -= method_0;
		}

		private void method_0(AbstractGame abstractGame_0, bool bool_0, int int_1, int int_2)
		{
			eGameType gameType = abstractGame_0.GameType;
			if ((uint)gameType <= 1u && int_2 == m_info.Para1 && base.Value < m_info.Para2)
			{
				base.Value++;
			}
			if (base.Value > m_info.Para2)
			{
				base.Value = m_info.Para2;
			}
		}
	}
}
