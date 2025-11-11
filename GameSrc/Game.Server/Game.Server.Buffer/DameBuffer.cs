using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Buffer
{
	public class DameBuffer : AbstractBuffer
	{
		public DameBuffer(BufferInfo buffer)
			: base(buffer)
		{
		}

		public override void Start(GamePlayer player)
		{
			DameBuffer ofType = player.BufferList.GetOfType(typeof(DameBuffer)) as DameBuffer;
			if (ofType != null)
			{
				ofType.Info.ValidDate += base.Info.ValidDate;
				if (ofType.Info.ValidDate > 30)
				{
					ofType.Info.ValidDate = 30;
				}
				player.BufferList.UpdateBuffer(ofType);
			}
			else
			{
				base.Start(player);
				player.PlayerCharacter.DameAddPlus += base.Info.Value;
			}
		}

		public override void Stop()
		{
			m_player.PlayerCharacter.DameAddPlus -= m_info.Value;
			base.Stop();
		}
	}
}
