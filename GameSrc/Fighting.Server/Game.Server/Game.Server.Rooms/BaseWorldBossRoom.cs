using Bussiness;
using Bussiness.CenterService;
using Bussiness.Managers;
using Game.Base;
using Game.Base.Packets;
using Game.Server;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;

namespace Game.Server.Rooms
{
    public class BaseWorldBossRoom
    {
        private Dictionary<int, GamePlayer> m_list;

        private long MAX_BLOOD = (long)350000000;

        private long m_blood;

        private string m_name;

        private string m_bossResourceId;

        private DateTime m_begin_time;

        private DateTime m_end_time;

        private int m_currentPVE;

        private bool m_fightOver;

        private bool m_roomClose;

        private bool m_worldOpen;

        private int m_fight_time;

        private bool m_die;

        public int playerDefaultPosX = 265;

        public int playerDefaultPosY = 1030;

        public int ticketID = 11573;

        public int need_ticket_count;

        public int timeCD = 15;

        public int reviveMoney = 10000;

        public int reFightMoney = 12000;

        public int addInjureBuffMoney = 30000;

        public int addInjureValue = 200;

        public DateTime Begin_time
        {
            get
            {
                return this.m_begin_time;
            }
        }

        public long Blood
        {
            get
            {
                return this.m_blood;
            }
            set
            {
                this.m_blood = value;
            }
        }

        public string BossResourceId
        {
            get
            {
                return this.m_bossResourceId;
            }
        }

        public int CurrentPVE
        {
            get
            {
                return this.m_currentPVE;
            }
        }

        public DateTime End_time
        {
            get
            {
                return this.m_end_time;
            }
        }

        public int Fight_time
        {
            get
            {
                return this.m_fight_time;
            }
        }

        public bool FightOver
        {
            get
            {
                return this.m_fightOver;
            }
        }

        public bool IsDie
        {
            get
            {
                return this.m_die;
            }
            set
            {
                this.m_die = value;
            }
        }

        public long MaxBlood
        {
            get
            {
                return this.MAX_BLOOD;
            }
        }

        public string Name
        {
            get
            {
                return this.m_name;
            }
        }

        public bool RoomClose
        {
            get
            {
                return this.m_roomClose;
            }
        }

        public bool WorldbossOpen
        {
            get
            {
                return this.m_worldOpen;
            }
        }

        public BaseWorldBossRoom()
        {
            this.m_list = new Dictionary<int, GamePlayer>();
            this.m_name = "Boss";
            this.m_bossResourceId = "0";
            this.m_currentPVE = 0;
        }

        public bool AddPlayer(GamePlayer player)
        {
            bool flag = false;
            lock (this.m_list)
            {
                if (!this.m_list.ContainsKey(player.PlayerId))
                {
                    player.IsInWorldBossRoom = true;
                    this.m_list.Add(player.PlayerId, player);
                    flag = true;
                    this.ShowRank();
                    this.SendPrivateInfo(player.PlayerCharacter.NickName);
                }
            }
            if (flag)
            {
                GSPacketIn gSPacketIn = new GSPacketIn(102);
                gSPacketIn.WriteByte(3);
                gSPacketIn.WriteInt(player.PlayerCharacter.Grade);
                gSPacketIn.WriteInt(player.PlayerCharacter.Hide);
                gSPacketIn.WriteInt(player.PlayerCharacter.Repute);
                gSPacketIn.WriteInt(player.PlayerCharacter.ID);
                gSPacketIn.WriteString(player.PlayerCharacter.NickName);
                gSPacketIn.WriteByte(player.PlayerCharacter.typeVIP);
                gSPacketIn.WriteInt(player.PlayerCharacter.VIPLevel);
                gSPacketIn.WriteBoolean(player.PlayerCharacter.Sex);
                gSPacketIn.WriteString(player.PlayerCharacter.Style);
                gSPacketIn.WriteString(player.PlayerCharacter.Colors);
                gSPacketIn.WriteString(player.PlayerCharacter.Skin);
                gSPacketIn.WriteInt(player.X);
                gSPacketIn.WriteInt(player.Y);
                gSPacketIn.WriteInt(player.PlayerCharacter.FightPower);
                gSPacketIn.WriteInt(player.PlayerCharacter.Win);
                gSPacketIn.WriteInt(player.PlayerCharacter.Total);
                gSPacketIn.WriteInt(player.PlayerCharacter.Offer);
                gSPacketIn.WriteByte(player.States);
                this.SendToALL(gSPacketIn);
            }
            return flag;
        }

        public void FightOverAll()
        {
            GSPacketIn gSPacketIn = new GSPacketIn(82);
            GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
        }

        public GamePlayer[] GetPlayersSafe()
        {
            GamePlayer[] gamePlayerArray = null;
            lock (this.m_list)
            {
                gamePlayerArray = new GamePlayer[this.m_list.Count];
                this.m_list.Values.CopyTo(gamePlayerArray, 0);
            }
            if (gamePlayerArray != null)
            {
                return gamePlayerArray;
            }
            return new GamePlayer[0];
        }

        public void ReduceBlood(int value)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(84);
            gSPacketIn.WriteInt(value);
            GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
        }

        public bool RemovePlayer(GamePlayer player)
        {
            bool flag = false;
            lock (this.m_list)
            {
                flag = this.m_list.Remove(player.PlayerId);
                GSPacketIn gSPacketIn = new GSPacketIn(102);
                gSPacketIn.WriteByte(4);
                gSPacketIn.WriteInt(player.PlayerId);
                this.SendToALL(gSPacketIn);
            }
            if (flag)
            {
                player.Out.SendSceneRemovePlayer(player);
            }
            return true;
        }

        public void SendFightOver()
        {
            GSPacketIn gSPacketIn = new GSPacketIn(102);
            gSPacketIn.WriteByte(8);
            gSPacketIn.WriteBoolean(true);
            this.SendToALLPlayers(gSPacketIn);
        }

        public void SendPrivateInfo(string name)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(85);
            gSPacketIn.WriteString(name);
            GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
        }

        public void SendPrivateInfo(string name, int damage, int honor)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(102);
            gSPacketIn.WriteByte(22);
            gSPacketIn.WriteInt(damage);
            gSPacketIn.WriteInt(honor);
            GamePlayer[] playersSafe = this.GetPlayersSafe();
            for (int i = 0; i < (int)playersSafe.Length; i++)
            {
                GamePlayer gamePlayer = playersSafe[i];
                if (gamePlayer.PlayerCharacter.NickName == name)
                {
                    gamePlayer.Out.SendTCP(gSPacketIn);
                    return;
                }
            }
        }

        public void SendRoomClose()
        {
            GSPacketIn gSPacketIn = new GSPacketIn(102);
            gSPacketIn.WriteByte(9);
            this.SendToALLPlayers(gSPacketIn);
        }

        public void SendToALL(GSPacketIn packet)
        {
            this.SendToALL(packet, null);
        }

        public void SendToALL(GSPacketIn packet, GamePlayer except)
        {
            GamePlayer[] gamePlayerArray = null;
            lock (this.m_list)
            {
                gamePlayerArray = new GamePlayer[this.m_list.Count];
                this.m_list.Values.CopyTo(gamePlayerArray, 0);
            }
            if (gamePlayerArray != null)
            {
                GamePlayer[] gamePlayerArray1 = gamePlayerArray;
                for (int i = 0; i < (int)gamePlayerArray1.Length; i++)
                {
                    GamePlayer gamePlayer = gamePlayerArray1[i];
                    if (gamePlayer != null && gamePlayer != except)
                    {
                        gamePlayer.Out.SendTCP(packet);
                    }
                }
            }
        }

        public void SendToALLPlayers(GSPacketIn packet)
        {
            GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
            for (int i = 0; i < (int)allPlayers.Length; i++)
            {
                allPlayers[i].SendTCP(packet);
            }
        }

        public void SendUpdateBlood(GSPacketIn packet)
        {
            long num = packet.ReadLong();
            this.m_blood = packet.ReadLong();
            GSPacketIn gSPacketIn = new GSPacketIn(102);
            gSPacketIn.WriteByte(5);
            gSPacketIn.WriteBoolean(false);
            gSPacketIn.WriteLong(num);
            gSPacketIn.WriteLong(this.m_blood);
            this.SendToALL(gSPacketIn);
        }

        public void ShowRank()
        {
            GSPacketIn gSPacketIn = new GSPacketIn(86);
            GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
        }

        public void UpdateRank(int damage, int honor, string nickName)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(81);
            gSPacketIn.WriteInt(damage);
            gSPacketIn.WriteInt(honor);
            gSPacketIn.WriteString(nickName);
            GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
        }

        public void UpdateWorldBoss(GSPacketIn pkg)
        {
            long num = pkg.ReadLong();
            long num1 = pkg.ReadLong();
            string str = pkg.ReadString();
            string str1 = pkg.ReadString();
            int num2 = pkg.ReadInt();
            bool flag = pkg.ReadBoolean();
            bool flag1 = pkg.ReadBoolean();
            this.m_begin_time = pkg.ReadDateTime();
            this.m_end_time = pkg.ReadDateTime();
            this.m_fight_time = pkg.ReadInt();
            bool flag2 = pkg.ReadBoolean();
            if (!this.m_worldOpen)
            {
                this.m_die = flag;
                this.m_fightOver = flag;
                this.m_roomClose = flag1;
                this.MAX_BLOOD = num;
                this.m_blood = num1;
                this.m_name = str;
                this.m_bossResourceId = str1;
                this.m_currentPVE = num2;
                this.m_worldOpen = flag2;
                GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
                for (int i = 0; i < (int)allPlayers.Length; i++)
                {
                    GamePlayer gamePlayer = allPlayers[i];
                    gamePlayer.Out.SendOpenWorldBoss(gamePlayer.X, gamePlayer.Y);
                }
            }
            if (this.m_fightOver && !this.m_die)
            {
                this.FightOverAll();
                this.m_die = true;
            }
        }

        public void UpdateWorldBossRankCrosszone(GSPacketIn packet)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(102);
            gSPacketIn.WriteByte(10);
            bool flag = packet.ReadBoolean();
            int num = packet.ReadInt();
            gSPacketIn.WriteBoolean(flag);
            gSPacketIn.WriteInt(num);
            for (int i = 0; i < num; i++)
            {
                int num1 = packet.ReadInt();
                string str = packet.ReadString();
                int num2 = packet.ReadInt();
                gSPacketIn.WriteInt(num1);
                gSPacketIn.WriteString(str);
                gSPacketIn.WriteInt(num2);
                if (this.m_fightOver)
                {
                    SendWorldBossRankAward(str, num1);
                }
            }
            if (flag)
            {
                this.SendToALLPlayers(gSPacketIn);
                return;
            }
            this.SendToALL(gSPacketIn);
        }

        private void SendWorldBossRankAward(string name, int rank)
        {
            List<ItemInfo> items = new List<ItemInfo>();
            WorldBossTopTenAwardInfo goods = AwardMgr.GetWorldBossAwardByID(rank);
            ItemTemplateInfo temp = ItemMgr.FindItemTemplate(goods.TemplateID);
            if (temp == null)
                return;
            ItemInfo item = ItemInfo.CreateFromTemplate(temp, 1, 102);
            item.IsBinds = goods.IsBinds;
            item.ValidDate = goods.Validate;
            item.StrengthenLevel = goods.StrengthenLevel;
            item.AttackCompose = goods.AttackCompose;
            item.DefendCompose = goods.DefendCompose;
            item.LuckCompose = goods.LuckCompose;
            item.AgilityCompose = goods.AgilityCompose;
            items.Add(item);
            PlayerBussiness pb = new PlayerBussiness();
            PlayerInfo info = pb.GetUserSingleByNickName(name);
            pb.SendItemsToMail(items, info.ID, GameServer.Instance.Configuration.ZoneId, $"Quà chiến đấu với Boss Tà Diệm Long bạn đã đạt được hạng {rank}!.", "Quà Boss Thế Giới");
            CenterServiceClient cs = new CenterServiceClient();
            cs.MailNotice(info.ID);
        }

        public void ViewOtherPlayerRoom(GamePlayer player)
        {
            GamePlayer[] playersSafe = this.GetPlayersSafe();
            for (int i = 0; i < (int)playersSafe.Length; i++)
            {
                GamePlayer gamePlayer = playersSafe[i];
                if (gamePlayer != player)
                {
                    GSPacketIn gSPacketIn = new GSPacketIn(102);
                    gSPacketIn.WriteByte(3);
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Grade);
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Hide);
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Repute);
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.ID);
                    gSPacketIn.WriteString(gamePlayer.PlayerCharacter.NickName);
                    gSPacketIn.WriteByte(gamePlayer.PlayerCharacter.typeVIP);
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.VIPLevel);
                    gSPacketIn.WriteBoolean(gamePlayer.PlayerCharacter.Sex);
                    gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Style);
                    gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Colors);
                    gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Skin);
                    gSPacketIn.WriteInt(gamePlayer.X);
                    gSPacketIn.WriteInt(gamePlayer.Y);
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.FightPower);
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Win);
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Total);
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Offer);
                    gSPacketIn.WriteByte(gamePlayer.States);
                    player.SendTCP(gSPacketIn);
                }
            }
        }

        public void WorldBossClose()
        {
            this.m_worldOpen = false;
            GamePlayer[] playersSafe = this.GetPlayersSafe();
            for (int i = 0; i < (int)playersSafe.Length; i++)
            {
                this.RemovePlayer(playersSafe[i]);
            }
        }
    }
}