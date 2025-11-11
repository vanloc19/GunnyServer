using Game.Logic;
using Game.Server.ConsortiaTask.Data;
using SqlDataProvider.Data;

namespace Game.Server.ConsortiaTask.Condition
{
	public class GameMonsterCondition : BaseConsortiaTaskCondition
	{
		public GameMonsterCondition(ConsortiaTaskUserDataInfo player, BaseConsortiaTask quest, ConsortiaTaskInfo info, int value)
			: base(player, quest, info, value)
		{
		}

		public override void AddTrigger(ConsortiaTaskUserDataInfo player)
		{
			player.Player.AfterKillingLiving += method_0;
		}

		public override void RemoveTrigger(ConsortiaTaskUserDataInfo player)
		{
			player.Player.AfterKillingLiving -= method_0;
		}

		private void method_0(AbstractGame abstractGame_0, int int_1, int int_2, bool bool_0, int int_3, bool isArea)
		{
			if (int_1 == 2 && int_2 == m_info.Para1 && !(base.Value >= m_info.Para2 || bool_0))
			{
				base.Value++;
			}
		}
	}
}
