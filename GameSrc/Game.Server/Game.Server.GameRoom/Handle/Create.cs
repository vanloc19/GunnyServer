using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.Rooms;
using System;

namespace Game.Server.GameRoom.Handle
{
    [GameRoomHandleAttbute((byte)eRoomPackageType.GAME_ROOM_CREATE)]
    public class Create : IGameRoomCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            byte roomType = packet.ReadByte();
            byte timeType = packet.ReadByte();
            string roomName = packet.ReadString();
            string pwd = packet.ReadString();

            if(roomType == 14)
            {
                if(!RoomMgr.WorldBossRoom.WorldbossOpen)
                {
                    Player.CurrentRoom.RemovePlayerUnsafe(Player);
                    return false;
                }
                if (DateTime.Compare(Player.LastEnterWorldBoss.AddSeconds(55.0), DateTime.Now) > 0)
                {
                    Player.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("Tốc độ client quá nhanh.", new object[0]));
                    return false;
                }
                Player.LastEnterWorldBoss = DateTime.Now;
                Player.WorldbossBood = RoomMgr.WorldBossRoom.Blood;
                AbstractBuffer abstractBuffer = BufferList.CreatePayBuffer(400, 50000, 1);
                if (abstractBuffer != null)
                {
                    abstractBuffer.Start(Player);
                }
                abstractBuffer = BufferList.CreatePayBuffer(406, 30000, 1);
                if (abstractBuffer != null)
                {
                    abstractBuffer.Start(Player);
                }
            }

            RoomMgr.CreateRoom(Player, roomName, pwd, (eRoomType)roomType, timeType);
            return true;
        }
    }
}