using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Games;

namespace Game.Server.Rooms
{
    public class CreateCryptBossAction : IAction
    {
        private GamePlayer m_player;
        private string m_name;
        private string m_password;
        private eRoomType m_roomType;
        private byte m_timeType;
        private int m_section;
        private int m_mapId;
        private eHardLevel m_hardLevel;

        public CreateCryptBossAction(GamePlayer player, int id, int level)
        {
            m_player = player;
            m_name = "CryptBoss";
            m_password = "12dasS12dasda44C@tchBe@st";
            m_roomType = eRoomType.CryptBoss;
            m_section = 0;
            m_timeType = 3;
            switch (id)
            {
                case 1:
                    m_mapId = 50201;
                    break;
                case 2:
                    m_mapId = 50202;
                    break;
                case 3:
                    m_mapId = 50203;
                    break;
                case 4:
                    m_mapId = 50204;
                    break;
                case 5:
                    m_mapId = 50205;
                    break;
                case 6:
                    m_mapId = 50206;
                    break;
                default:
                    m_mapId = 50201;
                    break;
            }

            switch (level)
            {
                case 1:
                    m_hardLevel = eHardLevel.Easy;
                    break;
                case 2:
                    m_hardLevel = eHardLevel.Normal;
                    break;
                case 3:
                    m_hardLevel = eHardLevel.Hard;
                    break;
                case 4:
                    m_hardLevel = eHardLevel.Terror;
                    m_timeType = 2;
                    break;
                case 5:
                    m_hardLevel = eHardLevel.Epic;
                    m_timeType = 1;
                    break;
                default:
                    m_hardLevel = eHardLevel.Easy;
                    break;
            }
        }

        public void Execute()
        {
            if (m_player.MainWeapon == null)
            {
                m_player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip"));
            }
            else
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
                    room.HardLevel = m_hardLevel;
                    room.LevelLimits = (int)room.GetLevelLimit(m_player);
                    room.isCrosszone = false;
                    room.isOpenBoss = false;
                    room.MapId = m_mapId;
                    room.currentFloor = m_section;
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
}