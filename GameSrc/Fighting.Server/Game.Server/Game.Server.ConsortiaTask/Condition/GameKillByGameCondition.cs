using Game.Logic;
using Game.Server.ConsortiaTask.Data;
using SqlDataProvider.Data;

namespace Game.Server.ConsortiaTask.Condition
{
	public class GameKillByGameCondition : BaseConsortiaTaskCondition
	{
		public GameKillByGameCondition(ConsortiaTaskUserDataInfo player, BaseConsortiaTask quest, ConsortiaTaskInfo info, int value)
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
			if (bool_0 || int_1 != 1)
			{
				return;
			}
			switch (abstractGame_0.GameType)
			{
				case eGameType.Free:
					if ((m_info.Para1 == 0 || m_info.Para1 == -1) && base.Value < m_info.Para2)
					{
						base.Value++;
					}
					break;
				case eGameType.Guild:
					if ((m_info.Para1 == 1 || m_info.Para1 == -1) && base.Value < m_info.Para2)
					{
						base.Value++;
					}
					break;
				case eGameType.Training:
					if ((m_info.Para1 == 2 || m_info.Para1 == -1) && base.Value < m_info.Para2)
					{
						base.Value++;
					}
					break;
				case eGameType.ALL:
					if ((m_info.Para1 == 4 || m_info.Para1 == -1) && base.Value < m_info.Para2)
					{
						base.Value++;
					}
					break;
				case eGameType.Exploration:
					if ((m_info.Para1 == 5 || m_info.Para1 == -1) && base.Value < m_info.Para2)
					{
						base.Value++;
					}
					break;
				case eGameType.Boss:
					if ((m_info.Para1 == 6 || m_info.Para1 == -1) && base.Value < m_info.Para2)
					{
						base.Value++;
					}
					break;
				case eGameType.Dungeon:
					if ((m_info.Para1 == 7 || m_info.Para1 == -1) && base.Value < m_info.Para2)
					{
						base.Value++;
					}
					break;
			}
			if (base.Value > m_info.Para2)
			{
				base.Value = m_info.Para2;
			}
		}
	}
}
