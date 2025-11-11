using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils
{
    public class PlayerFarm : PlayerFarmInventory
    {
        protected GamePlayer m_player;

        public GamePlayer Player
        {
            get { return m_player; }
        }

        private bool m_saveToDb;

        private bool m_loadFromDb;

        public PlayerFarm(GamePlayer player, bool saveTodb, int capibility, int beginSlot) : base(capibility, beginSlot)
        {
            m_player = player;
            m_saveToDb = saveTodb;
            m_loadFromDb = false;
        }

        public virtual void LoadFromDatabase()
        {
            if (m_saveToDb)
            {
                using (PlayerBussiness pb = new PlayerBussiness())
                {
                    UserFarmInfo farm = pb.GetSingleFarm(m_player.PlayerCharacter.ID);
                    UserFieldInfo[] fields = pb.GetSingleFields(m_player.PlayerCharacter.ID);
                    try
                    {
                        UpdateFarm(farm);
                        if (farm != null)
                        {
                            foreach (UserFieldInfo field in fields)
                            {
                                AddFieldTo(field, field.FieldID, farm.FarmID);
                            }
                        }
                    }
                    finally
                    {
                        if (m_farm == null)
                        {
                            CreateFarm(m_player.PlayerCharacter.ID, m_player.PlayerCharacter.NickName);
                        }
                    }
                }
            }
        }

        public virtual void SaveToDatabase()
        {
            if (m_saveToDb)
            {
                using (PlayerBussiness pb = new PlayerBussiness())
                {
                    lock (m_lock)
                    {
                        if (m_farm != null && m_farm.IsDirty)
                        {
                            if (m_farm.ID > 0)
                            {
                                pb.UpdateFarm(m_farm);
                            }
                            else
                            {
                                pb.AddFarm(m_farm);
                            }
                        }
                    }

                    lock (m_lock)
                    {
                        if (m_farm != null)
                        {
                            for (int i = 0; i < m_fields.Length; i++)
                            {
                                UserFieldInfo field = m_fields[i];
                                if (field != null && field.IsDirty)
                                {
                                    if (field.ID > 0)
                                    {
                                        pb.UpdateFields(field);
                                    }
                                    else
                                    {
                                        pb.AddFields(field);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool AddFieldTo(UserFieldInfo item, int place, int farmId)
        {
            if (base.AddFieldTo(item, place))
            {
                item.FarmID = farmId;
                return true;
            }

            return false;
        }

        public bool AddOtherFieldTo(UserFieldInfo item, int place, int farmId)
        {
            if (base.AddOtherFieldTo(item, place))
            {
                item.FarmID = farmId;
                return true;
            }

            return false;
        }

        public override void GropFastforward(bool isAllField, int fieldId)
        {
            base.GropFastforward(isAllField, fieldId);
            m_player.Out.SenddoMature(this);
        }

        #region Farmhandler
        public void EnterFarm(bool isEnter)
        {
            CropHelperSwitchField(false);
            if (isEnter)
            {
                m_player.Out.SendEnterFarm(m_player.PlayerCharacter, CurrentFarm, GetFields());
                m_farmstatus = 1;
            }
        }

        public void LoadFarmLand()
        {
            if (!m_loadFromDb)
            {
                LoadFromDatabase();
                m_loadFromDb = true;
                //m_player.Out.SendFarmLandInfo(this);
            }
        }

        public void CropHelperSwitchField(bool isStopFarmHelper)
        {
            if (m_farm != null)
            {
                if (m_farm.isFarmHelper)
                {
                    ItemTemplateInfo item = ItemMgr.FindItemTemplate(m_farm.isAutoId);
                    ItemTemplateInfo food = ItemMgr.FindItemTemplate(item.Property4);
                    ItemInfo cloneItem = ItemInfo.CreateFromTemplate(food, 1, 102);
                    int _timeDiff = 0;
                    int _autoTime = m_farm.AutoValidDate * 60;
                    TimeSpan usedTime = DateTime.Now - m_farm.AutoPayTime;
                    int KillCropId = m_farm.KillCropId;
                    if (usedTime.TotalMilliseconds < 0)
                    {
                        _timeDiff = _autoTime;
                    }
                    else
                    {
                        _timeDiff = _autoTime - (int)usedTime.TotalMilliseconds;
                    }

                    int currentCount = ((1 - _timeDiff / _autoTime) * KillCropId) / 1000;
                    if (currentCount > KillCropId)
                    {
                        currentCount = KillCropId;
                        isStopFarmHelper = true;
                    }

                    if (isStopFarmHelper)
                    {
                        cloneItem.Count = currentCount;
                        cloneItem.IsBinds = true;
                        if (currentCount > 0)
                        {
                            string content = string.Format("Kết thúc trợ thủ, bạn nhận được {0} {1}", currentCount, cloneItem.Template.Name);
                            string title = LanguageMgr.GetTranslation("Kết thúc trợ thủ, nhận được thức ăn thú cưng!");
                            m_player.SendItemToMail(cloneItem, content, title, eMailType.ItemOverdue);
                            m_player.Out.SendMailResponse(m_player.PlayerCharacter.ID, eMailRespose.Receiver);
                        }

                        lock (m_lock)
                        {
                            m_farm.isFarmHelper = false;
                            m_farm.isAutoId = 0;
                            m_farm.AutoPayTime = DateTime.Now;
                            m_farm.AutoValidDate = 0;
                            m_farm.GainFieldId = 0;
                            m_farm.KillCropId = 0;
                        }

                        m_player.Out.SendHelperSwitchField(m_player.PlayerCharacter, m_farm);
                    }
                }
            }
        }

        public void ExitFarm()
        {       
            m_farmstatus = 0;
        }

        public virtual void HelperSwitchField(bool isHelper, int seedID, int seedTime, int haveCount, int getCount)
        {
            if (base.HelperSwitchFields(isHelper, seedID, seedTime, haveCount, getCount))
            {
                m_player.Out.SendHelperSwitchField(m_player.PlayerCharacter, m_farm);
            }
        }

        public virtual bool GainFriendFields(int userId, int fieldId)
        {
            GamePlayer friend_player = WorldMgr.GetPlayerById(userId);
            UserFieldInfo field = m_otherFields[fieldId];
            if (field == null)
                return false;
            ItemTemplateInfo item = ItemMgr.FindItemTemplate(field.SeedID);
            ItemTemplateInfo food = ItemMgr.FindItemTemplate(item.Property4);
            ItemInfo cloneItem = ItemInfo.CreateFromTemplate(food, 1, 102);
            List<ItemInfo> overdueItems = new List<ItemInfo>();
            if (GetOtherFieldAt(fieldId).isDig())
                return false;
            lock (m_lock)
            {
                if (m_otherFields[fieldId].GainCount > 9)
                {
                    m_otherFields[fieldId].GainCount--;
                }
                else
                {
                    return false;
                }
            }

            if (!m_player.PropBag.StackItemToAnother(cloneItem))
            {
                if (!m_player.PropBag.AddItem(cloneItem))
                {
                    overdueItems.Add(cloneItem);
                }
            }

            if (friend_player == null)
            {
                using (PlayerBussiness pb = new PlayerBussiness())
                {
                    for (int i = 0; i < m_otherFields.Length; i++)
                    {
                        UserFieldInfo f = m_otherFields[i];
                        if (f != null)
                        {
                            pb.UpdateFields(f);
                        }
                    }
                }
            }
            else
            {
                if (friend_player.Farm.Status == 1)
                {
                    friend_player.Farm.UpdateGainCount(fieldId, m_otherFields[fieldId].GainCount);
                    friend_player.Out.SendtoGather(friend_player.PlayerCharacter, m_otherFields[fieldId]);
                }
            }

            m_player.Out.SendtoGather(m_player.PlayerCharacter, m_otherFields[fieldId]);
            if (overdueItems.Count > 0)
            {
                string mess = LanguageMgr.GetTranslation("Túi đầy");
                m_player.SendItemsToMail(overdueItems, mess, mess, eMailType.ItemOverdue);
                m_player.Out.SendMailResponse(m_player.PlayerCharacter.ID, eMailRespose.Receiver);
            }

            return true;
        }

        public void EnterFriendFarm(int userId)
        {
            GamePlayer otherPlayer = WorldMgr.GetPlayerById(userId);
            UserFarmInfo farm;
            UserFieldInfo[] fields;
            using (PlayerBussiness db = new PlayerBussiness())
            {
                if (otherPlayer == null)
                {
                    farm = db.GetSingleFarm(userId);
                    fields = db.GetSingleFields(userId);
                }
                else
                {
                    farm = otherPlayer.Farm.CurrentFarm;
                    fields = otherPlayer.Farm.CurrentFields;
                    otherPlayer.ViFarmsAdd(m_player.PlayerCharacter.ID);
                }

                if (farm == null)
                {
                    if (otherPlayer == null)
                    {
                        farm = CreateFarmForNulll(userId);
                        fields = CreateFieldsForNull(userId);
                    }
                    else
                    {
                        farm = db.GetSingleFarm(userId);
                        fields = db.GetSingleFields(userId);
                    }
                }
            }

            m_farmstatus = m_player.PlayerCharacter.ID;
            if (farm != null)
            {
                UpdateOtherFarm(farm);
                ClearOtherFields();
                foreach (UserFieldInfo field in fields)
                {
                    if (field != null)
                    {
                        AddOtherFieldTo(field, field.FieldID, farm.FarmID);
                    }
                }

                m_midAutumnFlag = false;
                m_player.Out.SendEnterFarm(m_player.PlayerCharacter, OtherFarm, GetOtherFields());
            }
        }

        public virtual void PayField(List<int> fieldIds, int payFieldTime)
        {
            if (base.CreateField(m_player.PlayerCharacter.ID, fieldIds, payFieldTime))
            {
                foreach (int id in m_player.ViFarms)
                {
                    GamePlayer p = WorldMgr.GetPlayerById(id);
                    if (p != null)
                    {
                        if (p.Farm.Status == id)
                        {
                            p.Out.SendPayFields(m_player, fieldIds);
                        }
                    }
                }

                m_player.Out.SendPayFields(m_player, fieldIds);
            }
        }

        public override bool GrowField(int fieldId, int templateID)
        {
            if (base.GrowField(fieldId, templateID))
            {
                foreach (int id in m_player.ViFarms)
                {
                    GamePlayer p = WorldMgr.GetPlayerById(id);
                    if (p != null)
                    {
                        if (p.Farm.Status == id)
                        {
                            p.Out.SendSeeding(m_player.PlayerCharacter, m_fields[fieldId]);
                        }
                    }
                }

                m_player.Out.SendSeeding(m_player.PlayerCharacter, m_fields[fieldId]);
                return true;
            }

            return false;
        }

        public override bool killCropField(int fieldId)
        {
            if (base.killCropField(fieldId))
            {
                m_player.Out.SendKillCropField(m_player.PlayerCharacter, m_fields[fieldId]);
                return true;
            }

            return false;
        }

        public virtual bool GainField(int fieldId)
        {
            if (fieldId < 0 || fieldId > GetFields().Count())
                return false;
            if (!CurrentFields[fieldId].isDig())
                return false;
            ItemTemplateInfo item = ItemMgr.FindItemTemplate(CurrentFields[fieldId].SeedID);
            if (item == null)
                return false;
            List<ItemInfo> overdueItems = new List<ItemInfo>();
            ItemTemplateInfo food = ItemMgr.FindItemTemplate(item.Property4);
            ItemInfo cloneItem = ItemInfo.CreateFromTemplate(food, CurrentFields[fieldId].GainCount, 102);
            cloneItem.IsBinds = true;
            if (base.killCropField(fieldId))
            {
                if (!m_player.AddTemplate(cloneItem))
                {
                    overdueItems.Add(cloneItem);
                }

                m_player.Out.SendtoGather(m_player.PlayerCharacter, CurrentFields[fieldId]);

                foreach (int id in m_player.ViFarms)
                {
                    GamePlayer p = WorldMgr.GetPlayerById(id);
                    if (p != null)
                    {
                        if (p.Farm.Status == id)
                        {
                            p.Out.SendtoGather(m_player.PlayerCharacter, CurrentFields[fieldId]);
                        }
                    }
                }

                m_player.OnCropPrimaryEvent();
                if (overdueItems.Count > 0)
                {
                    string mess = LanguageMgr.GetTranslation("Túi đầy");
                    m_player.SendItemsToMail(overdueItems, mess, mess, eMailType.ItemOverdue);
                    m_player.Out.SendMailResponse(m_player.PlayerCharacter.ID, eMailRespose.Receiver);
                }

                return true;
            }

            return false;
        }
        #endregion
    }
}