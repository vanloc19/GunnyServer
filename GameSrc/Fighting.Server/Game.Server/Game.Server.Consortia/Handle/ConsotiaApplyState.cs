using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Consortia.Handle
{
	[global::Consortia(7)]
	public class ConsotiaApplyState : IConsortiaCommandHadler
	{
		public int CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			if (Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			bool flag = packet.ReadBoolean();
			bool val = false;
			string msg = "CONSORTIA_APPLY_STATE.Failed";
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				if (consortiaBussiness.UpdateConsotiaApplyState(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, flag, ref msg))
				{
					msg = "CONSORTIA_APPLY_STATE.Success";
					val = true;
				}
			}
			GSPacketIn packet2 = new GSPacketIn(129);
			packet2.WriteByte(7);
			packet2.WriteBoolean(flag);
			packet2.WriteBoolean(val);
			packet2.WriteString(LanguageMgr.GetTranslation(msg));
			Player.Out.SendTCP(packet2);
			return 0;
		}
	}
}
