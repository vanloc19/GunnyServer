using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Buffer
{
	public class ConsortionAddEffectTurnBuffer : AbstractBuffer
	{
		public ConsortionAddEffectTurnBuffer(BufferInfo buffer)
			: base(buffer)
		{
		}

		public override void Start(GamePlayer player)
		{
			ConsortionAddEffectTurnBuffer ofType = player.BufferList.GetOfType(typeof(ConsortionAddEffectTurnBuffer)) as ConsortionAddEffectTurnBuffer;
			if (ofType != null)
			{
				ofType.Info.ValidDate += m_info.ValidDate;
				ofType.Info.TemplateID = m_info.TemplateID;
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
