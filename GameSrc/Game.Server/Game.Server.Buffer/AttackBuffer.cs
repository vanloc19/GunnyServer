using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Buffer
{
	public class AttackBuffer : AbstractBuffer
	{
		public AttackBuffer(BufferInfo buffer)
			: base(buffer)
		{
		}

		public override void Start(GamePlayer player)
		{
			AttackBuffer ofType = player.BufferList.GetOfType(typeof(AttackBuffer)) as AttackBuffer;
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
				player.PlayerCharacter.AttackAddPlus += base.Info.Value;
			}
		}

		public override void Stop()
		{
			m_player.PlayerCharacter.AttackAddPlus -= m_info.Value;
			base.Stop();
		}
	}
}
