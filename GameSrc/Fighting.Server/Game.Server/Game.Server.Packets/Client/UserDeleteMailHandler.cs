using Bussiness;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
	[PacketHandler((byte)ePackageType.DELETE_MAIL, "删除邮件")]
	public class UserDeleteMailHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			GSPacketIn @in = new GSPacketIn((byte)ePackageType.DELETE_MAIL, client.Player.PlayerCharacter.ID);
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("Bag.Locked"));
				return 0;
			}
			int val = packet.ReadInt();
			@in.WriteInt(val);
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				if (bussiness.DeleteMail(client.Player.PlayerCharacter.ID, val, out var num2))
				{
					client.Out.SendMailResponse(num2, eMailRespose.Receiver);
					@in.WriteBoolean(val: true);
				}
				else
				{
					@in.WriteBoolean(val: false);
				}
			}
			client.Out.SendTCP(@in);
			return 0;
		}
	}
}
