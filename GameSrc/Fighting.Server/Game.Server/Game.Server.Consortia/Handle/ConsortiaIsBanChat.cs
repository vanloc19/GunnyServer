using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Consortia.Handle
{
	[global::Consortia(16)]
	public class ConsortiaIsBanChat : IConsortiaCommandHadler
	{
		public int CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			if (Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			int num = packet.ReadInt();
			bool flag = packet.ReadBoolean();
			int tempID = 0;
			string tempName = "";
			bool val = false;
			string msg = "ConsortiaIsBanChatHandler.Failed";
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				if (consortiaBussiness.UpdateConsortiaIsBanChat(num, Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, flag, ref tempID, ref tempName, ref msg))
				{
					msg = "ConsortiaIsBanChatHandler.Success";
					val = true;
					GameServer.Instance.LoginServer.SendConsortiaBanChat(tempID, tempName, Player.PlayerCharacter.ID, Player.PlayerCharacter.NickName, flag);
				}
			}
			GSPacketIn packet2 = new GSPacketIn(129);
			packet2.WriteByte(16);
			packet2.WriteInt(num);
			packet2.WriteBoolean(flag);
			packet2.WriteBoolean(val);
			packet2.WriteString(LanguageMgr.GetTranslation(msg));
			Player.Out.SendTCP(packet2);
			return 0;
		}
	}
}
