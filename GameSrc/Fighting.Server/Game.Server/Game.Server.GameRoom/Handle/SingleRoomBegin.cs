using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.Rooms;
using System;

namespace Game.Server.GameRoom.Handle
{
    [GameRoomHandleAttbute((byte)eRoomPackageType.SINGLE_ROOM_BEGIN)]
    public class SingleRoomBegin : IGameRoomCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            int num = packet.ReadInt();
            if (Player.MainWeapon == null)
            {
                Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip"));
                return false;
            }
            switch (num)
            {
                case 4:
                    {
                        if (Player.CurrentRoom != null)
                        {
                            Player.CurrentRoom.RemovePlayerUnsafe(Player);
                        }
                        if (!Player.IsActive)
                            return false;
                        RoomMgr.WaitingRoom.RemovePlayer(Player);
                        RoomMgr.CreateConsortiaBattleRoom(Player);
                        break;
                    }
            }
            return true;
        }
    }
}