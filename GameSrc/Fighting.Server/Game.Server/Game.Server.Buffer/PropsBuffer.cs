using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Buffer
{
	public class PropsBuffer : AbstractBuffer
	{
		public PropsBuffer(BufferInfo buffer)
			: base(buffer)
		{
		}

		public override void Start(GamePlayer player)
		{
			PropsBuffer ofType = player.BufferList.GetOfType(typeof(PropsBuffer)) as PropsBuffer;
			if (ofType != null)
			{
				ofType.Info.ValidDate += base.Info.ValidDate;
				player.BufferList.UpdateBuffer(ofType);
			}
			else
			{
				base.Start(player);
				player.CanUseProp = true;
			}
		}

		public override void Stop()
		{
			m_player.CanUseProp = false;
			base.Stop();
		}
	}
}
