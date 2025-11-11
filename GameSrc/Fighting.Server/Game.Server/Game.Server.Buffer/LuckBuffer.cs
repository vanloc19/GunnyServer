using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Buffer
{
	public class LuckBuffer : AbstractBuffer
	{
		public LuckBuffer(BufferInfo buffer)
			: base(buffer)
		{
		}

		public override void Start(GamePlayer player)
		{
			LuckBuffer ofType = player.BufferList.GetOfType(typeof(LuckBuffer)) as LuckBuffer;
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
				player.PlayerCharacter.LuckAddPlus += base.Info.Value;
			}
		}

		public override void Stop()
		{
			m_player.PlayerCharacter.LuckAddPlus -= m_info.Value;
			base.Stop();
		}
	}
}
