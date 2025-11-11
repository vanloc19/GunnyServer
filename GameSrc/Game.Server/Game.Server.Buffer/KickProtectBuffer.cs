using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Buffer
{
	public class KickProtectBuffer : AbstractBuffer
	{
		public KickProtectBuffer(BufferInfo info)
			: base(info)
		{
		}

		public override void Start(GamePlayer player)
		{
			KickProtectBuffer ofType = player.BufferList.GetOfType(typeof(KickProtectBuffer)) as KickProtectBuffer;
			if (ofType != null)
			{
				ofType.Info.ValidDate += base.Info.ValidDate;
				player.BufferList.UpdateBuffer(ofType);
			}
			else
			{
				base.Start(player);
				player.KickProtect = true;
			}
		}

		public override void Stop()
		{
			m_player.KickProtect = false;
			base.Stop();
		}
	}
}
