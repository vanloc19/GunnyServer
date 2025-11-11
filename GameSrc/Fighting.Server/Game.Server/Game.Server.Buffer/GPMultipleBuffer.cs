using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Buffer
{
	public class GPMultipleBuffer : AbstractBuffer
	{
		public GPMultipleBuffer(BufferInfo info)
			: base(info)
		{
		}

		public override void Start(GamePlayer player)
		{
			GPMultipleBuffer ofType = player.BufferList.GetOfType(typeof(GPMultipleBuffer)) as GPMultipleBuffer;
			if (ofType != null)
			{
				ofType.Info.ValidDate += base.Info.ValidDate;
				player.BufferList.UpdateBuffer(ofType);
			}
			else
			{
				base.Start(player);
				player.GPAddPlus *= base.Info.Value;
			}
		}

		public override void Stop()
		{
			if (m_player != null)
			{
				m_player.GPAddPlus /= base.Info.Value;
				base.Stop();
			}
		}
	}
}
