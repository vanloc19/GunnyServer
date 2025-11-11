using Bussiness;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
	[PacketHandler(234, "撤消征婚信息")]
	public class MarryInfoDeleteHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int iD = packet.ReadInt();
			string translation = LanguageMgr.GetTranslation("MarryInfoDeleteHandler.Fail");
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				if (bussiness.DeleteMarryInfo(iD, client.Player.PlayerCharacter.ID, ref translation))
				{
					client.Out.SendAuctionRefresh(null, iD, isExist: false, null);
				}
				client.Out.SendMessage(eMessageType.GM_NOTICE, translation);
			}
			return 0;
		}
	}
}
