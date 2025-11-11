using System;
using System.Collections.Generic;
using System.Reflection;
using Bussiness;
using Bussiness.Managers;
using Game.Logic;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils
{
    public abstract class PlayerFarmInventory
    {
        protected object m_lock = new object();

        protected bool m_midAutumnFlag;

        private int m_capalility;

        private int m_beginSlot;

        protected UserFarmInfo m_farm;

        protected UserFieldInfo[] m_fields;

        protected UserFarmInfo m_otherFarm;

        protected UserFieldInfo[] m_otherFields;

        protected int m_farmstatus;

        public UserFarmInfo OtherFarm
        {
            get { return m_otherFarm; }
        }

        public UserFieldInfo[] OtherFields
        {
            get { return m_otherFields; }
        }

        public UserFarmInfo CurrentFarm
        {
            get { return m_farm; }
        }

        public UserFieldInfo[] CurrentFields
        {
            get { return m_fields; }
        }

        public int Status
        {
            get { return m_farmstatus; }
        }

        public PlayerFarmInventory(int capability, int beginSlot)
        {
            m_capalility = capability;
            m_beginSlot = beginSlot;
            m_fields = new UserFieldInfo[capability];
            m_otherFields = new UserFieldInfo[capability];
            m_farmstatus = 0;
        }

        public int ripeNum()
        {
            int num = 0;
            lock (m_lock)
            {
                for (int i = 0; i < m_fields.Length; i++)
                {
                    if (m_fields[i] != null && m_fields[i].SeedID != 0)
                        num++;
                }
            }

            return num;
        }

        public virtual void GropFastforward(bool isAllField, int fieldId)
        {
            lock (m_lock)
            {
                if (isAllField)
                {
                    for (int i = 0; i < m_fields.Length; i++)
                    {
                        if (m_fields[i] != null && m_fields[i].SeedID != 0)
                        {
                            m_fields[i].AccelerateTime += GameProperties.FastGrowSubTime;
                        }
                    }
                }
                else
                {
                    if (m_fields[fieldId] != null && m_fields[fieldId].SeedID != 0)
                    {
                        m_fields[fieldId].AccelerateTime += GameProperties.FastGrowSubTime;
                    }
                }
            }
        }

        #region Add/Remove/AddTemp/RemoveTemp

        public bool AddField(UserFieldInfo item)
        {
            return AddField(item, m_beginSlot);
        }

        public bool AddField(UserFieldInfo item, int minSlot)
        {
            if (item == null) return false;

            int place = FindFirstEmptySlot(minSlot);

            return AddFieldTo(item, place);
        }

        public virtual bool AddFieldTo(UserFieldInfo item, int place)
        {
            if (item == null || place >= m_capalility || place < 0) return false;

            lock (m_lock)
            {
                m_fields[place] = item;
                if (m_fields[place] != null)
                    place = -1;
                else
                {
                    m_fields[place] = item;
                    item.FieldID = place;
                }
            }

            return place != -1;
        }

        public virtual bool AddOtherFieldTo(UserFieldInfo item, int place)
        {
            if (item == null || place >= m_capalility || place < 0) return false;

            lock (m_lock)
            {
                m_otherFields[place] = item;
                if (m_otherFields[place] != null)
                    place = -1;
                else
                {
                    m_otherFields[place] = item;
                    item.FieldID = place;
                }
            }

            return place != -1;
        }

        public virtual bool RemoveOtherField(UserFieldInfo item)
        {
            if (item == null) return false;
            int place = -1;
            lock (m_lock)
            {
                for (int i = 0; i < m_capalility; i++)
                {
                    if (m_otherFields[i] == item)
                    {
                        place = i;
                        m_otherFields[i] = null;

                        break;
                    }
                }
            }

            return place != -1;
        }
        #endregion

        public virtual UserFieldInfo GetFieldAt(int slot)
        {
            if (slot < 0 || slot >= m_capalility) return null;

            return m_fields[slot];
        }

        public int FindFirstEmptySlot(int minSlot)
        {
            if (minSlot >= m_capalility) return -1;

            lock (m_lock)
            {
                for (int i = minSlot; i < m_capalility; i++)
                {
                    if (m_fields[i] == null)
                    {
                        return i;
                    }
                }

                return -1;
            }
        }


        public void ClearOtherFields()
        {
            lock (m_lock)
            {
                for (int i = m_beginSlot; i < m_capalility; i++)
                {
                    if (m_otherFields[i] != null)
                    {
                        RemoveOtherField(m_otherFields[i]);
                    }
                }
            }
        }

        public virtual void ResetFarmProp()
        {
            lock (m_lock)
            {
                if (m_farm != null)
                {
                    m_farm.isArrange = false;
                    m_farm.buyExpRemainNum = 20;
                }
            }
        }

        public virtual void ClearIsArrange()
        {
            lock (m_lock)
            {
                m_farm.isArrange = true;
            }
        }

        public virtual void UpdateGainCount(int fieldId, int count)
        {
            lock (m_lock)
            {
                m_fields[fieldId].GainCount = count;
            }
        }

        public virtual void UpdateFarm(UserFarmInfo farm)
        {
            lock (m_lock)
            {
                m_farm = farm;
            }
        }

        public virtual void UpdateOtherFarm(UserFarmInfo farm)
        {
            lock (m_lock)
            {
                m_otherFarm = farm;
            }
        }

        public virtual bool GrowField(int fieldId, int templateID)
        {
            ItemTemplateInfo Temp = ItemMgr.FindItemTemplate(templateID);
            lock (m_lock)
            {
                m_fields[fieldId].SeedID = Temp.TemplateID;
                m_fields[fieldId].PlantTime = DateTime.Now;
                m_fields[fieldId].GainCount = Temp.Property2;
                m_fields[fieldId].FieldValidDate = Temp.Property3;
            }

            return true;
        }

        public virtual bool killCropField(int fieldId)
        {
            lock (m_lock)
            {
                if (m_fields[fieldId] != null)
                {
                    m_fields[fieldId].SeedID = 0;
                    m_fields[fieldId].FieldValidDate = 1;
                    m_fields[fieldId].AccelerateTime = 0;
                    m_fields[fieldId].GainCount = 0;
                    return true;
                }
            }

            return false;
        }

        public virtual void CreateFarm(int ID, string name)
        {
            string PayFieldMoney = PetMgr.FindConfig("PayFieldMoney").Value;
            string PayAutoMoney = PetMgr.FindConfig("PayAutoMoney").Value;
            UserFarmInfo farm = new UserFarmInfo();
            farm.ID = 0;
            farm.FarmID = ID;
            farm.FarmerName = name;
            farm.isFarmHelper = false;
            farm.isAutoId = 0;
            farm.AutoPayTime = DateTime.Now;
            farm.AutoValidDate = 0;
            farm.GainFieldId = 0;
            farm.KillCropId = 0;
            farm.PayAutoMoney = PayAutoMoney;
            farm.PayFieldMoney = PayFieldMoney;
            farm.buyExpRemainNum = 20;
            farm.isArrange = false;
            farm.TreeLevel = 0;
            farm.TreeExp = 0;
            farm.LoveScore = 0;
            farm.MonsterExp = 0;
            farm.PoultryState = 0;
            farm.CountDownTime = DateTime.Now;
            farm.TreeCostExp = 0;
            UpdateFarm(farm);
            CreateNewField(ID, 0, 8);
        }

        public virtual bool HelperSwitchFields(bool isHelper, int seedID, int seedTime, int haveCount, int getCount)
        {
            if (isHelper)
            {
                lock (m_lock)
                {
                    for (int i = 0; i < m_fields.Length; i++)
                    {
                        if (m_fields[i] != null)
                        {
                            m_fields[i].SeedID = 0;
                            m_fields[i].FieldValidDate = 1;
                            m_fields[i].AccelerateTime = 0;
                            m_fields[i].GainCount = 0;
                        }
                    }
                }
            }

            lock (m_lock)
            {
                m_farm.isFarmHelper = isHelper;
                m_farm.isAutoId = seedID;
                m_farm.AutoPayTime = DateTime.Now;
                m_farm.AutoValidDate = seedTime;
                m_farm.GainFieldId = (getCount / 10); // -haveCount;
                m_farm.KillCropId = getCount;
            }

            return true;
        }

        public virtual void CreateNewField(int ID, int minslot, int maxslot)
        {
            for (int i = minslot; i < maxslot; i++)
            {
                UserFieldInfo field = new UserFieldInfo();
                field.ID = 0;
                field.FarmID = ID;
                field.FieldID = i;
                field.SeedID = 0;
                field.PayTime = DateTime.Now.AddYears(100);
                field.payFieldTime = 876000;
                field.PlantTime = DateTime.Now;
                field.GainCount = 0;
                field.FieldValidDate = 1;
                field.AccelerateTime = 0;
                field.AutomaticTime = DateTime.Now;
                field.IsExit = true;
                AddFieldTo(field, i);
            }
        }

        public virtual bool CreateField(int ID, List<int> fieldIds, int payFieldTime)
        {
            for (int i = 0; i < fieldIds.Count; i++)
            {
                int place = fieldIds[i];
                if (m_fields[place] == null)
                {
                    UserFieldInfo field = new UserFieldInfo();
                    field.FarmID = ID;
                    field.FieldID = place;
                    field.SeedID = 0;
                    field.PayTime = DateTime.Now.AddDays((payFieldTime / 24));
                    field.payFieldTime = payFieldTime;
                    field.PlantTime = DateTime.Now;
                    field.GainCount = 0;
                    field.FieldValidDate = 1;
                    field.AccelerateTime = 0;
                    field.AutomaticTime = DateTime.Now;
                    field.IsExit = true;
                    AddFieldTo(field, place);
                }
                else
                {
                    m_fields[place].PayTime = DateTime.Now.AddDays((payFieldTime / 24));
                    m_fields[place].payFieldTime = payFieldTime;
                }
            }

            return true;
        }

        public virtual UserFieldInfo[] GetFields()
        {
            List<UserFieldInfo> list = new List<UserFieldInfo>();
            lock (m_lock)
            {
                for (int i = 0; i < m_capalility; i++)
                {
                    if (m_fields[i] != null)
                    {
                        if (m_fields[i].IsValidField())
                        {
                            list.Add(m_fields[i]);
                        }
                    }
                }
            }

            return list.ToArray();
        }

        public virtual UserFieldInfo[] GetOtherFields()
        {
            List<UserFieldInfo> list = new List<UserFieldInfo>();
            lock (m_lock)
            {
                for (int i = 0; i < m_capalility; i++)
                {
                    if (m_otherFields[i] != null)
                    {
                        if (m_otherFields[i].IsValidField())
                        {
                            list.Add(m_otherFields[i]);
                        }
                    }
                }
            }

            return list.ToArray();
        }

        public virtual UserFieldInfo GetOtherFieldAt(int slot)
        {
            if (slot < 0 || slot >= m_capalility) return null;

            return m_otherFields[slot];
        }

        public virtual int GetEmptyCount(int minSlot)
        {
            if (minSlot < 0 || minSlot > m_capalility - 1) return 0;

            int count = 0;
            lock (m_lock)
            {
                for (int i = minSlot; i < m_capalility; i++)
                {
                    if (m_fields[i] == null)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public virtual int payFieldMoneyToWeek()
        {
            return int.Parse(m_farm.PayFieldMoney.Split('|')[0].Split(',')[1]);
        }

        public virtual int payFieldMoneyToMonth()
        {
            return int.Parse(m_farm.PayFieldMoney.Split('|')[1].Split(',')[1]);
        }

        public virtual int payFieldTimeToMonth()
        {
            return int.Parse(m_farm.PayFieldMoney.Split('|')[1].Split(',')[0]);
        }

        public virtual int payAutoTimeToMonth()
        {
            return int.Parse(m_farm.PayAutoMoney.Split('|')[1].Split(',')[0]);
        }

        public virtual UserFarmInfo CreateFarmForNulll(int ID)
        {
            UserFarmInfo farm = new UserFarmInfo();
            farm.FarmID = ID;
            farm.FarmerName = "Null";
            farm.isFarmHelper = false;
            farm.isAutoId = 0;
            farm.AutoPayTime = DateTime.Now;
            farm.AutoValidDate = 0;
            farm.GainFieldId = 0;
            farm.KillCropId = 0;
            farm.isArrange = true;
            return farm;
        }

        public virtual UserFieldInfo[] CreateFieldsForNull(int ID)
        {
            List<UserFieldInfo> CreateFields = new List<UserFieldInfo>();
            for (int i = 0; i < 8; i++)
            {
                UserFieldInfo field = new UserFieldInfo();
                field.FarmID = ID;
                field.FieldID = i;
                field.SeedID = 0;
                field.PayTime = DateTime.Now;
                field.payFieldTime = 365000;
                field.PlantTime = DateTime.Now;
                field.GainCount = 0;
                field.FieldValidDate = 1;
                field.AccelerateTime = 0;
                field.AutomaticTime = DateTime.Now;

                CreateFields.Add(field);
            }

            return CreateFields.ToArray();
        }
    }
}