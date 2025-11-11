using Game.Logic;
using Game.Server.ConsortiaTask.Data;
using SqlDataProvider.Data;

namespace Game.Server.ConsortiaTask.Condition
{
	public class GameMissionOverCondition : BaseConsortiaTaskCondition
	{
		public GameMissionOverCondition(ConsortiaTaskUserDataInfo player, BaseConsortiaTask quest, ConsortiaTaskInfo info, int value)
			: base(player, quest, info, value)
		{
		}

		public override void AddTrigger(ConsortiaTaskUserDataInfo player)
		{
			player.Player.MissionFullOver += method_0;
		}

		public override void RemoveTrigger(ConsortiaTaskUserDataInfo player)
		{
			player.Player.MissionFullOver -= method_0;
		}

		private void method_0(AbstractGame abstractGame_0, int int_1, bool bool_0, int int_2)
		{
			if ((int_1 == m_info.Para1 || m_info.Para1 == -1) && ((base.Value < m_info.Para2) ? true : false) && (bool_0 ? true : false))
			{
				base.Value++;
			}
		}
	}
}
