using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Buffer
{
	public class CaddyGoodsBuffer : AbstractBuffer
	{
		public CaddyGoodsBuffer(BufferInfo buffer)
			: base(buffer)
		{
		}

		public override void Start(GamePlayer player)
		{
			CaddyGoodsBuffer ofType = player.BufferList.GetOfType(typeof(CaddyGoodsBuffer)) as CaddyGoodsBuffer;
			if (ofType != null)
			{
				ofType.Info.ValidDate += base.Info.ValidDate;
				player.BufferList.UpdateBuffer(ofType);
			}
			else
			{
				base.Start(player);
			}
		}

		public override void Stop()
		{
			base.Stop();
		}
	}
}
