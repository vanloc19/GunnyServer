using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Consortia.Handle
{
	[global::Consortia(5)]
	public class ConsortiaTryinDel : IConsortiaCommandHadler
	{
		public int CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			bool val = false;
			string msg = "ConsortiaApplyAllyDeleteHandler.Failed";
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				if (consortiaBussiness.DeleteConsortiaApplyUsers(num, Player.PlayerCharacter.ID, Player.PlayerCharacter.ConsortiaID, ref msg))
				{
					msg = ((Player.PlayerCharacter.ID == 0) ? "ConsortiaApplyAllyDeleteHandler.Success" : "ConsortiaApplyAllyDeleteHandler.Success2");
					val = true;
				}
			}
			GSPacketIn packet2 = new GSPacketIn(129);
			packet2.WriteByte(5);
			packet2.WriteInt(num);
			packet2.WriteBoolean(val);
			packet2.WriteString(LanguageMgr.GetTranslation(msg));
			Player.Out.SendTCP(packet2);
			return 0;
		}
	}
}
