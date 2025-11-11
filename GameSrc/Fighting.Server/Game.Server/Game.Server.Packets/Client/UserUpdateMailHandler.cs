using System;
using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler((byte)ePackageType.UPDATE_MAIL, "修改邮件的已读未读标志")]
	public class UserUpdateMailHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			GSPacketIn @in = new GSPacketIn((byte)ePackageType.UPDATE_MAIL, client.Player.PlayerCharacter.ID);
			int mailID = packet.ReadInt();
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				MailInfo mailSingle = bussiness.GetMailSingle(client.Player.PlayerCharacter.ID, mailID);
				if (mailSingle != null && !mailSingle.IsRead)
				{
					mailSingle.IsRead = true;
					if (mailSingle.Type < 100)
					{
						mailSingle.ValidDate = 72;
						mailSingle.SendTime = DateTime.Now;
					}
					bussiness.UpdateMail(mailSingle, mailSingle.Money);
					@in.WriteBoolean(true);
				}
				else
				{
					@in.WriteBoolean(false);
				}
			}
			client.Out.SendTCP(@in);
			return 0;
		}
	}
}
