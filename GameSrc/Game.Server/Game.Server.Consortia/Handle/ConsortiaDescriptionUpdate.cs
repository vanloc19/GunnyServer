using System.Text;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Consortia.Handle
{
	[global::Consortia(14)]
	public class ConsortiaDescriptionUpdate : IConsortiaCommandHadler
	{
		public int CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			string str = packet.ReadString();
			if (Encoding.Default.GetByteCount(str) > 300)
			{
				Player.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("ConsortiaDescriptionUpdateHandler.Long"));
				return 1;
			}
			bool val = false;
			string msg = "ConsortiaDescriptionUpdateHandler.Failed";
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				if (consortiaBussiness.UpdateConsortiaDescription(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, str, ref msg))
				{
					msg = "ConsortiaDescriptionUpdateHandler.Success";
					val = true;
				}
			}
			GSPacketIn packet2 = new GSPacketIn(129);
			packet2.WriteByte(14);
			packet2.WriteString(str);
			packet2.WriteBoolean(val);
			packet2.WriteString(LanguageMgr.GetTranslation(msg));
			Player.Out.SendTCP(packet2);
			return 0;
		}
	}
}
