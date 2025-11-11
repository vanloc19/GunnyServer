using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Server.GMActives
{
    public abstract class IGMActive
    {
        protected GmActivityInfo m_gmActive;

        protected Dictionary<int, UserGmActivityInfo> m_userGmList;

        protected bool m_IsLockAuto;

        public IGMActive(GmActivityInfo gmActive, List<UserGmActivityInfo> tempUserList)
        {
            m_gmActive = gmActive;
            m_IsLockAuto = false;
            m_userGmList = new Dictionary<int, UserGmActivityInfo>();

            if (tempUserList != null)
            {
                foreach(UserGmActivityInfo user in tempUserList)
                {
                    if (!m_userGmList.ContainsKey(user.UserID))
                    {
                        SetUser(user);
                        m_userGmList.Add(user.UserID, user);
                    }
                        
                }
            }
        }

        public bool IsLockAuto
        {
            get
            {
                return m_IsLockAuto;
            }
            set
            {
                m_IsLockAuto = value;
            }
        }

        public GmActivityInfo Info
        {
            get
            {
                return m_gmActive;
            }
        }

        public abstract void Start();
        public abstract void Stop();
        public abstract void SetUser(UserGmActivityInfo user);
        public virtual void SetStatusPacket(GamePlayer player, GSPacketIn pkg)
        {
            UserGmActivityInfo gmInfo = GetPlayer(player);

            pkg.WriteInt(gmInfo.StatusList.Count);
            foreach (GMStatusInfo status in gmInfo.StatusList.Values)
            {
                //Console.WriteLine("status: " + status.StatusID + "|" + status.StatusValue);
                pkg.WriteInt(status.StatusID);
                pkg.WriteInt(status.StatusValue);
            }
        }

        public virtual UserGmActivityInfo GetPlayer(GamePlayer player)
        {
            lock(m_userGmList)
            {
                if (m_userGmList.ContainsKey(player.PlayerId))
                    return m_userGmList[player.PlayerId];
            }

            return AddPlayer(player);
        }

        public virtual UserGmActivityInfo AddPlayer(GamePlayer player)
        {
            UserGmActivityInfo newUser = new UserGmActivityInfo
            {
                UserID = player.PlayerId,
                UserName = player.PlayerCharacter.UserName,
                NickName = player.PlayerCharacter.NickName,
                VipLevel = player.PlayerCharacter.VIPLevel
            };

            SetUser(newUser);

            lock (m_userGmList)
            {
                if(!m_userGmList.ContainsKey(player.PlayerId))
                {
                    m_userGmList.Add(player.PlayerId, newUser);
                }
            }

            return newUser;
        }

        public virtual List<UserGmActivityInfo> GetAllPlayers()
        {
            List<UserGmActivityInfo> lists = new List<UserGmActivityInfo>();
            lock (m_userGmList)
            {
                foreach (UserGmActivityInfo gm in m_userGmList.Values)
                {
                    lists.Add(gm);
                }
            }

            return lists;
        }

        public virtual bool GetAward(GamePlayer player, string giftGroupId, int index)
        {
            return false;
        }

        public virtual void SetGetAward(GamePlayer player, string giftGroupId, int total)
        {
            UserGmActivityInfo userGm = GetPlayer(player);

            userGm.UpdateGiftReceive(giftGroupId, total);
        }
        
        public virtual void DoAction() { }

        public virtual List<UserGmActivityInfo> GetRankTop()
        {
            return GetAllPlayers().Where(a => a.Value >= int.Parse(m_gmActive.remain2)).OrderByDescending(a => a.Value).Take(m_gmActive.GiftsGroup.Count).ToList();
        }

    }
}
