using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Buffer
{
	public class DefendBuffer : AbstractBuffer
	{
		public DefendBuffer(BufferInfo buffer)
			: base(buffer)
		{
		}

		public override void Start(GamePlayer player)
		{
			DefendBuffer ofType = player.BufferList.GetOfType(typeof(DefendBuffer)) as DefendBuffer;
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
				player.PlayerCharacter.DefendAddPlus += base.Info.Value;
			}
		}

		public override void Stop()
		{
			m_player.PlayerCharacter.DefendAddPlus -= m_info.Value;
			base.Stop();
		}
	}
}
