using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Packets;
using System;
using System.Collections.Generic;

namespace Game.Server.Rooms
{
    public class BaseWaitingRoom
    {
        private Dictionary<int, GamePlayer> m_list;

        public BaseWaitingRoom()
        {
            m_list = new Dictionary<int, GamePlayer>();
        }


        public bool AddPlayer(GamePlayer player)
        {
            bool result = false;
            lock (m_list)
            {
                if (!m_list.ContainsKey(player.PlayerId))
                {
                    m_list.Add(player.PlayerId, player);
                    result = true;
                }
            }

            if (result)
            {
                GSPacketIn pkg = player.Out.SendSceneAddPlayer(player);
                SendToALL(pkg, player);
            }

            return result;
        }

        public bool RemovePlayer(GamePlayer player)
        {
            bool result = false;
            lock (m_list)
            {
                result = m_list.Remove(player.PlayerId);
            }

            if (result)
            {
                GSPacketIn pkg = player.Out.SendSceneRemovePlayer(player);
                SendToALL(pkg, player);
            }

            return true;
        }

        public void SendSceneUpdate(GamePlayer player)
        {
            GSPacketIn pkg = player.Out.SendSceneAddPlayer(player);
            SendToALL(pkg, player);
            GamePlayer[] players = GetPlayersSafe();
            foreach (GamePlayer p in players)
            {
                if (p != player)
                {
                    player.Out.SendSceneAddPlayer(p);
                }
            }
        }

        #region SendUpdateRoom OLD
        /*public void SendUpdateRoom(GamePlayer player)
        {
            List<BaseRoom> rooms = new List<BaseRoom>();
            if (player.PlayerState == ePlayerState.Dungeon)
            {
                rooms.AddRange(RoomMgr.GetAllPveRooms());
            }
            else
            {
                rooms.AddRange(RoomMgr.GetAllMatchRooms());
            }
            player.Out.SendUpdateRoomList(rooms);
        }*/
        #endregion
        public void SendUpdateRoom(List<GamePlayer> players, List<BaseRoom> rooms)
        {
            GSPacketIn pkg = null;
            foreach (GamePlayer p in players)
            {
                if (pkg == null)
                {
                    pkg = p.Out.SendUpdateRoomList(rooms);
                }
                else
                {
                    p.Out.SendTCP(pkg);
                }
            }
        }

        #region OLD SendUpdateCurrentRoom
        /*public void SendUpdateCurrentRoom(BaseRoom room)
        {
            if (room != null)
            {
                List<BaseRoom> rooms = RoomMgr.GetAllRooms(room);
                GSPacketIn pkg = null;
                foreach (GamePlayer p in room.GetPlayers())
                {
                    if (pkg == null)
                    {
                        pkg = p.Out.SendUpdateRoomList(rooms);
                    }
                    else
                    {
                        p.Out.SendTCP(pkg);
                    }
                }
            }
        }*/
        #endregion
        public void SendUpdateCurrentRoom(BaseRoom room)
        {
            if (room != null)
            {
                GamePlayer[] playersSafe = this.GetPlayersSafe();
                List<BaseRoom> allRooms = RoomMgr.GetAllRooms();
                List<GamePlayer> playerpvp = new List<GamePlayer>();
                List<GamePlayer> playerpve = new List<GamePlayer>();
                List<BaseRoom> roompvp = new List<BaseRoom>();
                List<BaseRoom> roompve = new List<BaseRoom>();

                GamePlayer[] array = playersSafe;
                for (int i = 0; i < array.Length; i++)
                {
                    GamePlayer gamePlayer = array[i];
                    if (gamePlayer.PlayerState == ePlayerState.RoomList)
                    {
                        playerpvp.Add(gamePlayer);
                    }
                    if (gamePlayer.PlayerState == ePlayerState.Dungeon)
                    {
                        playerpve.Add(gamePlayer);
                    }
                }
                foreach (BaseRoom current in allRooms)
                {
                    if (current.RoomType == eRoomType.Freedom || current.RoomType == eRoomType.Match || current.RoomType == eRoomType.EliteGameScore || current.RoomType == eRoomType.EliteGameChampion)
                    {
                        roompvp.Add(current);
                    }
                    if (current.RoomType == eRoomType.Dungeon || current.RoomType == eRoomType.Academy)
                    {
                        roompve.Add(current);
                    }
                }
                SendUpdateRoom(playerpvp, roompvp);
                SendUpdateRoom(playerpve, roompve);
            }
        }

        public void SendToALL(GSPacketIn packet)
        {
            SendToALL(packet, null);
        }

        public void SendToALL(GSPacketIn packet, GamePlayer except)
        {
            GamePlayer[] temp = null;

            lock (m_list)
            {
                temp = new GamePlayer[m_list.Count];
                m_list.Values.CopyTo(temp, 0);
            }

            foreach (GamePlayer p in temp)
            {
                if (p != null && p != except)
                {
                    p.Out.SendTCP(packet);
                }
            }
        }

        public GamePlayer[] GetPlayersSafe()
        {
            GamePlayer[] temp = null;

            lock (m_list)
            {
                temp = new GamePlayer[m_list.Count];
                m_list.Values.CopyTo(temp, 0);
            }

            return temp;
        }
    }
}