using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Consortia.Handle
{
	[global::Consortia(13)]
	public class ConsortiaInviteDelete : IConsortiaCommandHadler
	{
		public int CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			bool val = false;
			string translateId = "ConsortiaInviteDeleteHandler.Failed";
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				if (consortiaBussiness.DeleteConsortiaInviteUsers(num, Player.PlayerCharacter.ID))
				{
					translateId = "ConsortiaInviteDeleteHandler.Success";
					val = true;
				}
			}
			GSPacketIn packet2 = new GSPacketIn(129);
			packet2.WriteByte(13);
			packet2.WriteInt(num);
			packet2.WriteBoolean(val);
			packet2.WriteString(LanguageMgr.GetTranslation(translateId));
			Player.Out.SendTCP(packet2);
			return 0;
		}
	}
}
