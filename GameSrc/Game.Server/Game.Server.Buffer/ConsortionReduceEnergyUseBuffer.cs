using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Buffer
{
	public class ConsortionReduceEnergyUseBuffer : AbstractBuffer
	{
		public ConsortionReduceEnergyUseBuffer(BufferInfo buffer)
			: base(buffer)
		{
		}

		public override void Start(GamePlayer player)
		{
			ConsortionReduceEnergyUseBuffer ofType = player.BufferList.GetOfType(typeof(ConsortionReduceEnergyUseBuffer)) as ConsortionReduceEnergyUseBuffer;
			if (ofType != null)
			{
				ofType.Info.ValidDate += m_info.ValidDate;
				ofType.Info.TemplateID = m_info.TemplateID;
				ofType.Info.Value = m_info.Value;
				player.BufferList.UpdateBuffer(ofType);
				player.UpdateFightBuff(base.Info);
			}
			else
			{
				base.Start(player);
				player.FightBuffs.Add(base.Info);
			}
		}

		public override void Stop()
		{
			m_player.FightBuffs.Remove(base.Info);
			base.Stop();
		}
	}
}
