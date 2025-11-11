using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils
{
    public class PlayerAvataInventory
    {
        protected object m_lock = new object();
        protected GamePlayer m_player;

        public GamePlayer Player
        {
            get { return m_player; }
        }

        private Dictionary<int, UserAvatarColectionInfo> m_avatarColect;

        public Dictionary<int, UserAvatarColectionInfo> AvatarColect
        {
            get { return m_avatarColect; }
        }

        public PlayerAvataInventory(GamePlayer player)
        {
            m_player = player;
            m_avatarColect = new Dictionary<int, UserAvatarColectionInfo>();
        }

        public virtual void LoadFromDatabase()
        {
            using (PlayerBussiness pb = new PlayerBussiness())
            {
                UserAvatarColectionInfo[] colects = pb.GetSingleUserAvatarColectionInfo(Player.PlayerCharacter.ID);
                if (colects.Length > 0)
                {
                    foreach (UserAvatarColectionInfo prop in colects)
                    {
                        if (AvatarColectionMgr.FindClothPropertyTemplate(prop.dataId) == null)
                        {
                            continue;
                        }

                        foreach (var VARIABLE in prop.CurrentGroup)
                        {
                            if (AvatarColectionMgr.FindClothGroupTemplateInfo(prop.dataId, Convert.ToInt32(VARIABLE), prop.Sex) == null)
                            {
                                goto Repeat;
                            }
                        }

                        AddAvatarColectionInfo(prop.dataId, prop.Sex, prop);
                    Repeat:;
                    }
                }

                CheckNewAvatarColection();
                CheckInvalidStyle();
            }
        }

        public virtual void SaveToDatabase()
        {
            using (PlayerBussiness pb = new PlayerBussiness())
            {
                lock (m_lock)
                {
                    foreach (UserAvatarColectionInfo info in m_avatarColect.Values)
                    {
                        if (info != null && info.IsDirty)
                        {
                            if (info.ID > 0)
                                pb.UpdateUserAvatarColectionInfo(info);
                            else
                                pb.AddUserAvatarColectionInfo(info);
                        }
                    }
                }
            }
        }

        public void UpdateInfo()
        {
            AvatarColectionInfoChange();
        }

        public int CountInGroup(int groupId)
        {
            return AvatarColectionMgr.FindClothGroupTemplateInfo(groupId).Count;
        }

        public void CheckNewAvatarColection()
        {
            List<ClothPropertyTemplateInfo> clothPropertys = AvatarColectionMgr.GetClothPropertyTemplate();
            foreach (ClothPropertyTemplateInfo prop in clothPropertys)
            {
                if (!AvatarColect.ContainsKey(prop.ID))
                {
                    UserAvatarColectionInfo info = new UserAvatarColectionInfo
                    {
                        UserID = m_player.PlayerCharacter.ID,
                        endTime = DateTime.Now.AddDays(-1),
                        dataId = prop.ID,
                        Sex = prop.Sex,
                        ActiveCount = CountInGroup(prop.ID),
                        ActiveDress = ""
                    };
                    AddAvatarColectionInfo(prop.ID, prop.Sex, info);
                }
            }
        }

        public void AddAvatarColectionInfo(int dataId, int sex, UserAvatarColectionInfo info)
        {
            if (AvatarColectionMgr.FindClothPropertyTemplate(dataId) == null)
            {
                Console.WriteLine("ClothPropertyTemplate.dataId: {0}, do not exit!", dataId);
                return;
            }

            lock (m_lock)
            {
                if (!m_avatarColect.ContainsKey(dataId))
                {
                    m_avatarColect.Add(dataId, info);
                }
            }
        }

        public void AvatarColectionInfoChange()
        {
            Player.Out.SendAvatarColectionAllInfo(Player.PlayerId, AvatarColect);
        }

        public void AddPropAvatarColection(ref int att, ref int def, ref int agi, ref int luk, ref int hp)
        {
            List<ClothPropertyTemplateInfo> clothPropertys = AvatarColectionMgr.GetClothPropertyTemplate();
            foreach (ClothPropertyTemplateInfo info in clothPropertys)
            {
                if (AvatarColect.ContainsKey(info.ID) && AvatarColect[info.ID].isValidate)
                {
                    if (AvatarColect[info.ID].FullActive())
                    {
                        att += info.Attack * 2;
                        def += info.Defend * 2;
                        agi += info.Agility * 2;
                        luk += info.Luck * 2;
                        hp += info.Blood * 2;
                    }
                    else if (AvatarColect[info.ID].HaftActive())
                    {
                        att += info.Attack;
                        def += info.Defend;
                        agi += info.Agility;
                        luk += info.Luck;
                        hp += info.Blood;
                    }
                }
            }
        }

        public void AddBasePropAvatarColection(ref double avatarGuard, ref double avatarDame)
        {
            List<ClothPropertyTemplateInfo> clothPropertys = AvatarColectionMgr.GetClothPropertyTemplate();

            foreach (ClothPropertyTemplateInfo info in clothPropertys)
            {
                if (AvatarColect.ContainsKey(info.ID) && AvatarColect[info.ID].isValidate)
                {
                    if (AvatarColect[info.ID].FullActive())
                    {
                        avatarGuard += info.Guard * 2;
                        avatarDame += info.Damage * 2;
                    }
                    else if (AvatarColect[info.ID].HaftActive())
                    {
                        avatarGuard += info.Guard;
                        avatarDame += info.Damage;
                    }
                }
            }
        }

        public UserAvatarColectionInfo GetAvatar(int dataId)
        {
            if (AvatarColect.ContainsKey(dataId))
            {
                return AvatarColect[dataId];
            }

            return null;
        }

        public bool ActiveAvatarColection(int dataId, List<int> ids, int sex, int type)
        {
            lock (m_lock)
            {
                foreach (int id in ids)
                {
                    ItemInfo item = Player.EquipBag.GetItemByTemplateID(0, id);
                    if (item != null)
                    {
                        if (m_avatarColect.ContainsKey(dataId))
                        {
                            string strArr = m_avatarColect[dataId].ActiveDress;

                            if (strArr.IndexOf(id.ToString(), StringComparison.Ordinal) != -1)
                                return false;

                            if (string.IsNullOrEmpty(strArr))
                            {
                                List<ClothGroupTemplateInfo> groups =
                                    AvatarColectionMgr.FindClothGroupTemplateInfo(dataId);
                                if (GameProperties.VERSION.ToString().Contains("404"))
                                    m_avatarColect[dataId].ActiveDress = FindDressActiveInBag(groups);

                                if (m_avatarColect[dataId].ActiveCount < groups.Count)
                                {
                                    m_avatarColect[dataId].ActiveCount = groups.Count;
                                }

                                m_avatarColect[dataId].ActiveDress = id.ToString();
                            }
                            else
                            {
                                m_avatarColect[dataId].ActiveDress = strArr + "," + id;
                            }

                            item.IsBinds = true;
                            Player.EquipBag.UpdateItem(item);
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        public bool ActiveAvatarColection(int dataId, int id)
        {
            lock (m_lock)
            {
                if (m_avatarColect.ContainsKey(dataId))
                {
                    string strArr = m_avatarColect[dataId].ActiveDress;

                    if (strArr.IndexOf(id.ToString(), StringComparison.Ordinal) != -1)
                        return false;
                    if (string.IsNullOrEmpty(strArr))
                    {
                        List<ClothGroupTemplateInfo> groups = AvatarColectionMgr.FindClothGroupTemplateInfo(dataId);
                        if (GameProperties.VERSION.ToString().Contains("404"))
                            m_avatarColect[dataId].ActiveDress = FindDressActiveInBag(groups);

                        if (m_avatarColect[dataId].ActiveCount < groups.Count)
                        {
                            m_avatarColect[dataId].ActiveCount = groups.Count;
                        }

                        m_avatarColect[dataId].ActiveDress = id.ToString();
                    }
                    else
                    {
                        m_avatarColect[dataId].ActiveDress = strArr + "," + id;
                    }

                    return true;
                }
            }

            return false;
        }

        public bool DelayAvatarColection(int dataId, int delay, int type)
        {
            if (AvatarColect.ContainsKey(dataId))
            {
                lock (m_lock)
                {
                    if (!AvatarColect[dataId].isValidate)
                    {
                        m_avatarColect[dataId].endTime = DateTime.Now;
                    }

                    m_avatarColect[dataId].endTime = AvatarColect[dataId].endTime.AddDays(delay);
                    if (m_avatarColect[dataId].FullActive() || m_avatarColect[dataId].HaftActive())
                    {
                        Player.EquipBag.UpdatePlayerProperties();
                    }

                    return true;
                }
            }

            return false;
        }

        public string FindDressActiveInBag(List<ClothGroupTemplateInfo> groups)
        {
            List<int> colect = new List<int>();
            foreach (ClothGroupTemplateInfo info in groups)
            {
                ItemInfo item = m_player.EquipBag.GetItemByTemplateID(0, info.TemplateID);
                if (item != null && !colect.Contains(item.TemplateID))
                {
                    colect.Add(item.TemplateID);
                }
            }

            if (colect.Count == 0)
            {
                return "";
            }

            string arrGroup = colect.Aggregate("", (current, id) => current + ("," + id));
            return arrGroup.Substring(1);
        }

        private void CheckInvalidStyle()
        {
            var style = m_player.PlayerCharacter.Style.Split(',');
            var styleCopy = m_player.PlayerCharacter.Style.Split(',');
            for (int x = 0; x < styleCopy.Length; x++)
            {
                var itemId = styleCopy[x].Split('|')[0];
                if (string.IsNullOrEmpty(itemId))
                {
                    continue;
                }

                if (ItemMgr.FindItemTemplate(Convert.ToInt32(itemId)) == null)
                {
                    style[x] = "";
                }
            }

            m_player.PlayerCharacter.Style = string.Join(",", style);
        }
    }
}