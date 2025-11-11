using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Consortia.Handle
{
	[global::Consortia(24)]
	public class ConsortiaEquipControl : IConsortiaCommandHadler
	{
		public int CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			if (Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			bool val1 = false;
			string msg = "ConsortiaEquipControlHandler.Fail";
			ConsortiaEquipControlInfo info = new ConsortiaEquipControlInfo();
			info.ConsortiaID = Player.PlayerCharacter.ConsortiaID;
			List<int> intList = new List<int>();
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				for (int index = 0; index < 5; index++)
				{
					info.Riches = packet.ReadInt();
					info.Type = 1;
					info.Level = index + 1;
					consortiaBussiness.AddAndUpdateConsortiaEuqipControl(info, Player.PlayerCharacter.ID, ref msg);
					intList.Add(info.Riches);
				}
				info.Riches = packet.ReadInt();
				info.Type = 2;
				info.Level = 0;
				intList.Add(info.Riches);
				consortiaBussiness.AddAndUpdateConsortiaEuqipControl(info, Player.PlayerCharacter.ID, ref msg);
				info.Riches = packet.ReadInt();
				info.Type = 3;
				info.Level = 0;
				intList.Add(info.Riches);
				consortiaBussiness.AddAndUpdateConsortiaEuqipControl(info, Player.PlayerCharacter.ID, ref msg);
				val1 = true;
			}
			GSPacketIn packet2 = new GSPacketIn(129);
			packet2.WriteByte(24);
			packet2.WriteBoolean(val1);
			foreach (int val2 in intList)
			{
				packet2.WriteInt(val2);
			}
			Player.Out.SendTCP(packet2);
			return 0;
		}
	}
}
