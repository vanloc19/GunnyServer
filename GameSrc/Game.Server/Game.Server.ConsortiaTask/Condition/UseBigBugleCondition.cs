using Game.Server.ConsortiaTask.Data;
using SqlDataProvider.Data;

namespace Game.Server.ConsortiaTask.Condition
{
	public class UseBigBugleCondition : BaseConsortiaTaskCondition
	{
		public UseBigBugleCondition(ConsortiaTaskUserDataInfo player, BaseConsortiaTask quest, ConsortiaTaskInfo info, int value)
			: base(player, quest, info, value)
		{
		}

		public override void AddTrigger(ConsortiaTaskUserDataInfo player)
		{
			player.Player.UseBugle += method_0;
		}

		public override void RemoveTrigger(ConsortiaTaskUserDataInfo player)
		{
			player.Player.UseBugle -= method_0;
		}

		private void method_0(int int_1)
		{
			if (int_1 == m_info.Para1 && base.Value < m_info.Para2)
			{
				base.Value++;
			}
		}
	}
}
