using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Buffer
{
	public class OfferMultipleBuffer : AbstractBuffer
	{
		public OfferMultipleBuffer(BufferInfo info)
			: base(info)
		{
		}

		public override void Start(GamePlayer player)
		{
			OfferMultipleBuffer ofType = player.BufferList.GetOfType(typeof(OfferMultipleBuffer)) as OfferMultipleBuffer;
			if (ofType != null)
			{
				ofType.Info.ValidDate += base.Info.ValidDate;
				player.BufferList.UpdateBuffer(ofType);
			}
			else
			{
				base.Start(player);
				player.OfferAddPlus *= base.Info.Value;
			}
		}

		public override void Stop()
		{
			m_player.OfferAddPlus /= m_info.Value;
			base.Stop();
		}
	}
}
