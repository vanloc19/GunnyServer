using Game.Logic;
using Game.Server.ConsortiaTask.Data;
using SqlDataProvider.Data;

namespace Game.Server.ConsortiaTask.Condition
{
	public class GameFightByRoomCondition : BaseConsortiaTaskCondition
	{
		public GameFightByRoomCondition(ConsortiaTaskUserDataInfo player, BaseConsortiaTask quest, ConsortiaTaskInfo info, int value)
			: base(player, quest, info, value)
		{
		}

		public override void AddTrigger(ConsortiaTaskUserDataInfo player)
		{
			player.Player.GameOver += method_0;
		}

		public override void RemoveTrigger(ConsortiaTaskUserDataInfo player)
		{
			player.Player.GameOver -= method_0;
		}

		private void method_0(AbstractGame abstractGame_0, bool bool_0, int int_1, bool isArea, bool isCouple)
		{
			switch (abstractGame_0.RoomType)
			{
			case eRoomType.Match:
				if ((m_info.Para1 == 0 || m_info.Para1 == -1) && base.Value < m_info.Para2)
				{
					base.Value++;
				}
				break;
			case eRoomType.Freedom:
				if ((m_info.Para1 == 1 || m_info.Para1 == -1) && base.Value < m_info.Para2)
				{
					base.Value++;
				}
				break;
			case eRoomType.Exploration:
				if ((m_info.Para1 == 2 || m_info.Para1 == -1) && base.Value < m_info.Para2)
				{
					base.Value++;
				}
				break;
			case eRoomType.Boss:
				if ((m_info.Para1 == 3 || m_info.Para1 == -1) && base.Value < m_info.Para2)
				{
					base.Value++;
				}
				break;
			case eRoomType.Dungeon:
				if ((m_info.Para1 == 4 || m_info.Para1 == -1) && base.Value < m_info.Para2)
				{
					base.Value++;
				}
				break;
			case eRoomType.Freshman:
				if ((m_info.Para1 == 2 || m_info.Para1 == -1) && base.Value < m_info.Para2)
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
