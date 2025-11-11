using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.Rooms;
using SqlDataProvider.Data;
using System;

namespace Game.Server.GameRoom.Handle
{
    [GameRoomHandleAttbute((byte)eRoomPackageType.GAME_ROOM_SETUP_CHANGE)]
    public class SetupChange : IGameRoomCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            if (Player.CurrentRoom != null && Player == Player.CurrentRoom.Host && !Player.CurrentRoom.IsPlaying)
            {
                int mapId = packet.ReadInt();
                eRoomType roomType = (eRoomType)packet.ReadByte();
                bool isOpenBoss = packet.ReadBoolean(); //_loc12_.writeBoolean(param3);
                string pic = "";
                string roomPass = packet.ReadString();
                string roomName = packet.ReadString();
                byte timeType = packet.ReadByte();
                eHardLevel hardLevel = (eHardLevel)packet.ReadByte();
                int levelLimits = packet.ReadInt();
                bool isCrossZone = packet.ReadBoolean();
                int mapId2 = packet.ReadInt();//_loc12_.writeInt(param10);
                int currentFloor = 1;
                if (mapId == 0 && roomType == eRoomType.Labyrinth)
                {
                    mapId = 401;
                    currentFloor = Player.Labyrinth.currentFloor;
                }
                if (mapId == 10000 && roomType == eRoomType.Dungeon)
                {
                    mapId = mapId2;
                }
                if (roomType == eRoomType.Dungeon && mapId != 10000)
                {
                    PveInfo pve = PveInfoMgr.GetPveInfoById(mapId);
                    if (pve != null && isOpenBoss)
                    {
                        int price = pve.getPrice((int)hardLevel);
                        if (Player.MoneyDirect(price, IsAntiMult: false) && Player.CheckOpenBoss())
                        {
                            var list = Player.CurrentRoom.GetPlayers();
                            Player.Extra.Info.BuyCountOpenBoss--;
                            Player.SendMessage(string.Format("Mở ải thành công.Ngày hôm nay còn {0} lần", Player.Extra.Info.BuyCountOpenBoss));
                            foreach (GamePlayer gamePlayer in list)
                            {
                                gamePlayer.IsMoAiCuoi = true;                 
                            }                  
                        }
                        else
                        {
                            Player.SendMessage("Số lượt ngày hôm nay đã hết hoặc không đủ xu!");
                            return false;
                        }
                    }
                }
                RoomMgr.UpdateRoomGameType(Player.CurrentRoom, roomType, timeType, (eHardLevel)hardLevel, levelLimits, mapId, roomPass, roomName, isCrossZone, isOpenBoss, pic, currentFloor);
            }
            return true;
        }
    }
}