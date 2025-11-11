using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Buffer
{
	public class GuardBuffer : AbstractBuffer
	{
		public GuardBuffer(BufferInfo buffer)
			: base(buffer)
		{
		}

		public override void Start(GamePlayer player)
		{
			GuardBuffer ofType = player.BufferList.GetOfType(typeof(GuardBuffer)) as GuardBuffer;
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
				player.PlayerCharacter.GuardAddPlus += base.Info.Value;
			}
		}

		public override void Stop()
		{
			m_player.PlayerCharacter.GuardAddPlus -= m_info.Value;
			base.Stop();
		}
	}
}
