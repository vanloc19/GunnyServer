using System;
using System.Collections.Generic;
using System.Drawing;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;

namespace Game.Server.Rooms
{
    public class BaseChristmasRoom
    {
        public static Random random = new Random();
        public static int LIVIN = 0;
        public static int DEAD = 2;
        public static int FIGHTING = 1;
        public static int MonterAddCount = 15;
        protected int lastMonterID = 1000;
        private int[] monterType = { 0, 1, 2 };

        private Point[] brithPoint =
        {
            new Point(353, 570),
            new Point(246, 760),
            new Point(593, 590),
            new Point(466, 898),
            new Point(800, 950),
            new Point(946, 748),
            new Point(1152, 873),
            new Point(1172, 874),
            new Point(1766, 630),
            new Point(1342, 581),
            new Point(1732, 401),
            new Point(1462, 326),
            new Point(1187, 207),
            new Point(878, 236),
            new Point(1590, 521)
        };

        private Dictionary<int, GamePlayer> m_players;
        private Dictionary<int, MonterInfo> m_monters;

        public Dictionary<int, MonterInfo> Monters
        {
            get { return m_monters; }
        }

        public int DefaultPosX = 500;
        public int DefaultPosY = 500;

        public BaseChristmasRoom()
        {
            m_players = new Dictionary<int, GamePlayer>();
            m_monters = new Dictionary<int, MonterInfo>();
            AddFistMonters();
        }

        public void AddFistMonters()
        {
            lock (m_monters)
            {
                for (var i = 0; i < MonterAddCount; i++)
                {
                    MonterInfo monter = new MonterInfo();
                    monter.ID = lastMonterID;
                    monter.type = random.Next(monterType.Length);
                    monter.MonsterPos = brithPoint[i];
                    monter.MonsterNewPos = brithPoint[i];
                    monter.state = LIVIN;
                    monter.PlayerID = 0;
                    if (!m_monters.ContainsKey(monter.ID))
                    {
                        m_monters.Add(monter.ID, monter);
                    }

                    lastMonterID++;
                }
            }
        }

        private int GetFreeMonter()
        {
            int count = 0;
            foreach (MonterInfo monter in Monters.Values)
            {
                if (monter.state == LIVIN)
                {
                    count++;
                }
            }

            return count;
        }

        public void AddMoreMonters()
        {
            if (GetFreeMonter() < m_players.Count)
            {
                AddMonters();
            }
        }

        public void AddMonters()
        {
            lock (m_monters)
            {
                MonterInfo monter = new MonterInfo();
                monter.ID = lastMonterID;
                monter.type = random.Next(monterType.Length);
                int index = random.Next(brithPoint.Length);
                monter.MonsterPos = brithPoint[index];
                monter.MonsterNewPos = brithPoint[index];
                monter.state = LIVIN;
                monter.PlayerID = 0;
                if (!m_monters.ContainsKey(monter.ID))
                {
                    m_monters.Add(monter.ID, monter);
                }

                lastMonterID++;
            }
        }

        public bool SetFightMonter(int Id, int playerId)
        {
            bool result = false;
            lock (m_monters)
            {
                if (m_monters.ContainsKey(Id))
                {
                    m_monters[Id].state = FIGHTING;
                    m_monters[Id].PlayerID = playerId;
                    result = true;
                }
            }

            AddMonters();

            return result;
        }

        public void SetMonterDie(int playerId)
        {
            int _id = -1;
            foreach (MonterInfo monter in m_monters.Values)
            {
                if (monter.PlayerID == playerId)
                {
                    _id = monter.ID;
                    break;
                }
            }

            if (_id > -1)
            {
                lock (m_monters)
                {
                    if (m_monters.ContainsKey(_id))
                    {
                        m_monters.Remove(_id);
                        //Console.WriteLine("m_monter: " + _id);
                    }
                }

                GSPacketIn response = new GSPacketIn(145);
                response.WriteByte((byte)ActiveSystemPackageType.CHRISTMAS_MONSTER);
                response.WriteByte(1); //type                        
                response.WriteInt(_id); //_loc_16 = _loc_3.readInt();hasKey
                SendToALL(response);
            }
        }

        public void AddPlayer(GamePlayer player)
        {
            lock (m_players)
            {
                if (!m_players.ContainsKey(player.PlayerId))
                {
                    m_players.Add(player.PlayerId, player);
                    player.Actives.BeginChristmasTimer();
                }
            }

            UpdateRoom();
        }

        public void UpdateRoom()
        {
            GamePlayer[] players = GetPlayersSafe();
            GSPacketIn pkg = new GSPacketIn(145);
            pkg.WriteByte((byte)ActiveSystemPackageType.ADDPLAYER);
            pkg.WriteInt(players.Length);
            foreach (GamePlayer p in players)
            {
                pkg.WriteInt(p.PlayerCharacter.Grade);
                pkg.WriteInt(p.PlayerCharacter.Hide);
                pkg.WriteInt(p.PlayerCharacter.Repute);
                pkg.WriteInt(p.PlayerCharacter.ID);
                pkg.WriteString(p.PlayerCharacter.NickName);
                pkg.WriteByte(p.PlayerCharacter.typeVIP);
                pkg.WriteInt(p.PlayerCharacter.VIPLevel);
                pkg.WriteBoolean(p.PlayerCharacter.Sex);
                pkg.WriteString(p.PlayerCharacter.Style);
                pkg.WriteString(p.PlayerCharacter.Colors);
                pkg.WriteString(p.PlayerCharacter.Skin);
                pkg.WriteInt(p.PlayerCharacter.FightPower);
                pkg.WriteInt(p.PlayerCharacter.Win);
                pkg.WriteInt(p.PlayerCharacter.Total);
                pkg.WriteInt(p.PlayerCharacter.Offer);
                pkg.WriteInt(p.X);
                pkg.WriteInt(p.Y);
                pkg.WriteByte(p.States);
            }

            SendToALL(pkg);
        }

        public void ViewOtherPlayerRoom(GamePlayer player)
        {
            GamePlayer[] players = GetPlayersSafe();
            GSPacketIn pkg = new GSPacketIn(145);
            pkg.WriteByte((byte)ActiveSystemPackageType.ADDPLAYER);
            pkg.WriteInt(players.Length);
            foreach (GamePlayer p in players)
            {
                pkg.WriteInt(p.PlayerCharacter.Grade);
                pkg.WriteInt(p.PlayerCharacter.Hide);
                pkg.WriteInt(p.PlayerCharacter.Repute);
                pkg.WriteInt(p.PlayerCharacter.ID);
                pkg.WriteString(p.PlayerCharacter.NickName);
                pkg.WriteByte(p.PlayerCharacter.typeVIP);
                pkg.WriteInt(p.PlayerCharacter.VIPLevel);
                pkg.WriteBoolean(p.PlayerCharacter.Sex);
                pkg.WriteString(p.PlayerCharacter.Style);
                pkg.WriteString(p.PlayerCharacter.Colors);
                pkg.WriteString(p.PlayerCharacter.Skin);
                pkg.WriteInt(p.PlayerCharacter.FightPower);
                pkg.WriteInt(p.PlayerCharacter.Win);
                pkg.WriteInt(p.PlayerCharacter.Total);
                pkg.WriteInt(p.PlayerCharacter.Offer);
                pkg.WriteInt(p.X);
                pkg.WriteInt(p.Y);
                pkg.WriteByte(p.States);
            }

            player.SendTCP(pkg);
        }

        public bool RemovePlayer(GamePlayer player)
        {
            bool result = false;
            lock (m_players)
            {
                player.Actives.StopChristmasTimer();
                result = m_players.Remove(player.PlayerId);
            }

            if (result)
            {
                GSPacketIn response = new GSPacketIn(145);
                response.WriteByte((byte)ActiveSystemPackageType.CHRISTMAS_EXIT);
                response.WriteInt(player.PlayerId);
                SendToALL(response);
                player.Out.SendSceneRemovePlayer(player);
            }

            return result;
        }

        public GamePlayer[] GetPlayersSafe()
        {
            GamePlayer[] temp = null;

            lock (m_players)
            {
                temp = new GamePlayer[m_players.Count];
                m_players.Values.CopyTo(temp, 0);
            }

            return temp == null ? new GamePlayer[0] : temp;
        }

        public void SendToALLPlayers(GSPacketIn packet)
        {
            GamePlayer[] players = WorldMgr.GetAllPlayers();
            foreach (GamePlayer p in players)
            {
                p.SendTCP(packet);
            }
        }

        public void SendToALL(GSPacketIn packet)
        {
            SendToALL(packet, (GamePlayer)null);
        }

        public void SendToALL(GSPacketIn packet, GamePlayer except)
        {
            GamePlayer[] temp = GetPlayersSafe();
            if (temp != null)
            {
                foreach (GamePlayer p in temp)
                {
                    if (p != null && p != except)
                    {
                        p.Out.SendTCP(packet);
                    }
                }
            }
        }
    }
}