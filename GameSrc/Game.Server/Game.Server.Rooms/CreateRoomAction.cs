using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Managers;
using System;

namespace Game.Server.Rooms
{
    public class CreateRoomAction : IAction
    {
        private GamePlayer m_player;

        private string m_name;

        private string m_password;

        private eRoomType m_roomType;

        private byte m_timeType;

        private Random rand;

        public CreateRoomAction(GamePlayer player, String name, String password, eRoomType roomType, byte timeType)
        {
            m_player = player;
            m_name = name;
            m_password = password;
            m_roomType = roomType;
            m_timeType = timeType;
            rand = new Random();
        }

        public void Execute()
        {
            if (m_player.CurrentRoom != null)
            {
                m_player.CurrentRoom.RemovePlayerUnsafe(m_player);
            }
            if (m_player.IsActive == false)
            {
                return;
            }
            BaseRoom[] rooms = RoomMgr.Rooms;
            BaseRoom room = null;
            int roomId = rand.Next(rooms.Length);
            for (int i = 0; i < rooms.Length; i++)
            {
                if (!rooms[roomId].IsUsing)
                {
                    room = rooms[roomId];
                    break;
                }
                roomId = rand.Next(rooms.Length);
            }

            if (room != null)
            {
                RoomMgr.WaitingRoom.RemovePlayer(m_player);
                room.Start();
                if (m_roomType == eRoomType.Dungeon)
                {
                    room.HardLevel = eHardLevel.Normal;
                    room.LevelLimits = (int)room.GetLevelLimit(m_player);
                    room.isOpenBoss = false;
                    room.currentFloor = 0;
                }

                if(WorldMgr.IsLeagueOpen)
                {
                    room.isWithinLeageTime = true;
                }

                room.UpdateRoom(m_name, m_password, m_roomType, m_timeType, 0);
                m_player.Out.SendRoomCreate(room);
                room.AddPlayerUnsafe(m_player);
                RoomMgr.WaitingRoom.SendUpdateCurrentRoom(room);
            }
        }
    }
}