using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Buffer
{
	public class TrainGoodsBuffer : AbstractBuffer
	{
		public TrainGoodsBuffer(BufferInfo buffer)
			: base(buffer)
		{
		}

		public override void Start(GamePlayer player)
		{
			TrainGoodsBuffer ofType = player.BufferList.GetOfType(typeof(TrainGoodsBuffer)) as TrainGoodsBuffer;
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
