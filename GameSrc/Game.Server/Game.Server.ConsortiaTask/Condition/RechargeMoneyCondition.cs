using Game.Server.ConsortiaTask.Data;
using SqlDataProvider.Data;

namespace Game.Server.ConsortiaTask.Condition
{
	public class RechargeMoneyCondition : BaseConsortiaTaskCondition
	{
		public RechargeMoneyCondition(ConsortiaTaskUserDataInfo player, BaseConsortiaTask quest, ConsortiaTaskInfo info, int value)
			: base(player, quest, info, value)
		{
		}

		public override void AddTrigger(ConsortiaTaskUserDataInfo player)
		{
			player.Player.MoneyCharge += method_0;
		}

		public override void RemoveTrigger(ConsortiaTaskUserDataInfo player)
		{
			player.Player.MoneyCharge -= method_0;
		}

		private void method_0(int int_1)
		{
			base.Value += int_1;
			if (base.Value > m_info.Para2)
			{
				base.Value = m_info.Para2;
			}
		}
	}
}
