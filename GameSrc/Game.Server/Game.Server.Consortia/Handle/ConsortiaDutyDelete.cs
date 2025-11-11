using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Consortia.Handle
{
	[global::Consortia(9)]
	public class ConsortiaDutyDelete : IConsortiaCommandHadler
	{
		public int CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			if (Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			int num = packet.ReadInt();
			bool val = false;
			string msg = "ConsortiaDutyDeleteHandler.Failed";
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				if (consortiaBussiness.DeleteConsortiaDuty(num, Player.PlayerCharacter.ID, Player.PlayerCharacter.ConsortiaID, ref msg))
				{
					msg = "ConsortiaDutyDeleteHandler.Success";
					val = true;
				}
			}
			GSPacketIn packet2 = new GSPacketIn(129);
			packet2.WriteByte(9);
			packet2.WriteInt(num);
			packet2.WriteBoolean(val);
			packet2.WriteString(LanguageMgr.GetTranslation(msg));
			Player.Out.SendTCP(packet2);
			return 0;
		}
	}
}
