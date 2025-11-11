using System.Collections.Generic;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Games;

namespace Game.Server.Rooms
{
    public class CreateCatchBeastAction : IAction
    {
        private GamePlayer m_player;

        private string m_name;

        private string m_password;

        private eRoomType m_roomType;

        private byte m_timeType;

        private int m_bossLevel;

        private int m_mapId;

        public CreateCatchBeastAction(GamePlayer player)
        {
            m_player = player;
            m_name = "CatchBeast";
            m_password = "12dasSda44C@tchBe@st";
            m_roomType = eRoomType.CatchBeast;
            m_timeType = 2;
            m_bossLevel = 1;
            m_mapId = (int)eMap.CatchBeast;
        }

        public void Execute()
        {
            if (m_player.CurrentRoom != null)
            {
                m_player.CurrentRoom.RemovePlayerUnsafe(m_player);
            }

            if (m_player.IsActive == false)
                return;
            BaseRoom[] rooms = RoomMgr.Rooms;

            BaseRoom room = null;

            for (int i = 0; i < rooms.Length; i++)
            {
                if (!rooms[i].IsUsing)
                {
                    room = rooms[i];
                    break;
                }
            }

            if (room != null)
            {
                RoomMgr.WaitingRoom.RemovePlayer(m_player);
                room.Start();
                room.HardLevel = eHardLevel.Normal;
                room.LevelLimits = (int)room.GetLevelLimit(m_player);
                room.isCrosszone = false;
                room.isOpenBoss = false;
                room.MapId = m_mapId;
                room.currentFloor = m_bossLevel;
                room.TimeMode = m_timeType;
                room.UpdateRoom(m_name, m_password, m_roomType, m_timeType, room.MapId);
                GSPacketIn pkg = m_player.Out.SendRoomCreate(room);
                if (room.AddPlayerUnsafe(m_player))
                {
                    List<GamePlayer> players = room.GetPlayers();
                    List<IGamePlayer> matchPlayers = new List<IGamePlayer>();
                    foreach (GamePlayer p in players)
                    {
                        if (p != null)
                        {
                            matchPlayers.Add(p);
                        }
                    }

                    BaseGame game = GameMgr.StartPVEGame(room.RoomId, matchPlayers, room.MapId, room.RoomType,
                        room.GameType, room.TimeMode, room.HardLevel, room.LevelLimits, room.currentFloor);
                    if (game != null)
                    {
                        room.IsPlaying = true;
                        room.StartGame(game);
                    }
                    else
                    {
                        room.IsPlaying = false;
                        room.SendPlayerState();
                    }
                }
            }
        }
    }
}