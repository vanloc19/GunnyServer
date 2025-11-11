using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Managers;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(70, "邀请")]
	public class GameInviteHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.CurrentRoom != null)
			{
				GamePlayer playerById = WorldMgr.GetPlayerById(packet.ReadInt());
				if (playerById == client.Player)
				{
					return 0;
				}
				GSPacketIn @in = new GSPacketIn(70, client.Player.PlayerCharacter.ID);
				foreach (GamePlayer player in client.Player.CurrentRoom.GetPlayers())
				{
					if (player == playerById)
					{
						client.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, LanguageMgr.GetTranslation("friendnotinthesameserver.Sameroom"));
						return 0;
					}
				}
				if (playerById != null && playerById.CurrentRoom == null)
				{
					@in.WriteInt(client.Player.PlayerCharacter.ID);
					@in.WriteInt(client.Player.CurrentRoom.RoomId);
					@in.WriteInt(client.Player.CurrentRoom.MapId);
					@in.WriteByte(client.Player.CurrentRoom.TimeMode);
					@in.WriteByte((byte)client.Player.CurrentRoom.RoomType);
					@in.WriteByte((byte)client.Player.CurrentRoom.HardLevel);
					@in.WriteByte((byte)client.Player.CurrentRoom.LevelLimits);
					@in.WriteString(client.Player.PlayerCharacter.NickName);
					@in.WriteBoolean((client.Player.PlayerCharacter.typeVIP > 0) ? true : false);
					@in.WriteInt(client.Player.PlayerCharacter.VIPLevel);
					@in.WriteString(client.Player.CurrentRoom.Name);
					@in.WriteString(client.Player.CurrentRoom.Password);
					@in.WriteInt(client.Player.CurrentRoom.barrierNum);
					@in.WriteBoolean(client.Player.CurrentRoom.isOpenBoss);
					playerById.Out.SendTCP(@in);
					/*if (client.Player.CurrentRoom.RoomType == eRoomType.Dungeon)
					{
						if (client.Player.CurrentRoom.Game != null)
						{
							@in.WriteInt((client.Player.CurrentRoom.Game as PVEGame).SessionId);
						}
						else
						{
							@in.WriteInt(0);
						}
					}
					else
					{
						@in.WriteInt(-1);
					}*/
				}
				else if (playerById != null && playerById.CurrentRoom != null && playerById.CurrentRoom != client.Player.CurrentRoom)
				{
					client.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, LanguageMgr.GetTranslation("friendnotinthesameserver.Room"));
				}
				else
				{
					client.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, LanguageMgr.GetTranslation("friendnotinthesameserver.Fail"));
				}
			}
			return 0;
		}
	}
}
