using System.Text;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Consortia.Handle
{
	[global::Consortia(15)]
	public class ConsortiaPlacardUpdate : IConsortiaCommandHadler
	{
		public int CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			string str = packet.ReadString();
			if (Encoding.Default.GetByteCount(str) > 300)
			{
				Player.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("ConsortiaPlacardUpdateHandler.Long"));
				return 1;
			}
			bool val = false;
			string msg = "ConsortiaPlacardUpdateHandler.Failed";
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				if (consortiaBussiness.UpdateConsortiaPlacard(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, str, ref msg))
				{
					msg = "ConsortiaPlacardUpdateHandler.Success";
					val = true;
				}
			}
			GSPacketIn packet2 = new GSPacketIn(129);
			packet2.WriteByte(15);
			packet2.WriteString(str);
			packet2.WriteBoolean(val);
			packet2.WriteString(LanguageMgr.GetTranslation(msg));
			Player.Out.SendTCP(packet2);
			return 0;
		}
	}
}
