using System;

namespace SqlDataProvider.Data
{
    public class UsersExtraInfo : DataObject
    {
        private int int_0;
        private DateTime dateTime_0;
        private DateTime dateTime_1;
        private int int_1;
        private DateTime dateTime_2;
        private int int_2;
        private int int_3;
        private int int_4;
        private int int_5;
        private DateTime dateTime_3;
        private int int_6;
        private int int_7;
        private int int_8;
        private bool bool_0;
        private bool bool_1;
        private int int_9;
        private int int_10;
        private int int_11;
        private int int_12;
        private bool bool_2;
        private int int_13;
        private int int_14;
        private int int_15;
        private int int_16;
        private int int_17;
        private int int_18;
        private DateTime dateTime_4;
        private int int_19;
        private int int_20;
        private float float_0;
        private int _BuyTimeHotSpringCount;
        private int _BuyCountOpenBoss;
        private int _coupleNum;
        private int _propsNum;
        private int _dungeonNum;

        private int _freeSendMailCount;

        public int UserID
        {
            get => this.int_0;
            set
            {
                this.int_0 = value;
                this._isDirty = true;
            }
        }

        public string NickName { get; set; }

        public DateTime LastTimeHotSpring
        {
            get => this.dateTime_0;
            set
            {
                this.dateTime_0 = value;
                this._isDirty = true;
            }
        }

        public DateTime LastFreeTimeHotSpring
        {
            get => this.dateTime_1;
            set
            {
                this.dateTime_1 = value;
                this._isDirty = true;
            }
        }

        public int MinHotSpring
        {
            get => this.int_1;
            set
            {
                this.int_1 = value;
                this._isDirty = true;
            }
        }

        public int BuyTimeHotSpringCount
        {
            get => this._BuyTimeHotSpringCount;
            set
            {
                this._BuyTimeHotSpringCount = value;
                this._isDirty = true;
            }
        }

        public int BuyCountOpenBoss
        {
            get => this._BuyCountOpenBoss;
            set
            {
                _BuyCountOpenBoss = value;
                _isDirty = true;
            }
        }

        public int coupleBossEnterNum
        {
            get => this.int_2;
            set
            {
                this.int_2 = value;
                this._isDirty = true;
            }
        }

        public int coupleBossHurt
        {
            get => this.int_3;
            set
            {
                this.int_3 = value;
                this._isDirty = true;
            }
        }

        public int coupleBossBoxNum
        {
            get => this.int_4;
            set
            {
                this.int_4 = value;
                this._isDirty = true;
            }
        }

        public int TotalCaddyOpen
        {
            get => this.int_5;
            set
            {
                this.int_5 = value;
                this._isDirty = true;
            }
        }

        public bool isGetAwardMarry
        {
            get => this.bool_0;
            set
            {
                this.bool_0 = value;
                this._isDirty = true;
            }
        }

        public bool isFirstAwardMarry
        {
            get => this.bool_1;
            set
            {
                this.bool_1 = value;
                this._isDirty = true;
            }
        }

        public int LeftRoutteCount
        {
            get => this.int_20;
            set
            {
                this.int_20 = value;
                this._isDirty = true;
            }
        }

        public float LeftRoutteRate
        {
            get => this.float_0;
            set
            {
                this.float_0 = value;
                this._isDirty = true;
            }
        }

        public int FreeSendMailCount
        {
            get { return _freeSendMailCount; }
            set
            {
                _freeSendMailCount = value;
                _isDirty = true;
            }
        }

        public int coupleNum
        {
            get { return _coupleNum; }
            set
            {
                _coupleNum = value;
                _isDirty = true;
            }
        }

        public int propsNum
        {
            get { return _propsNum; }
            set
            {
                _propsNum = value;
                _isDirty = true;
            }
        }

        public int dungeonNum
        {
            get { return _dungeonNum; }
            set
            {
                _dungeonNum = value;
                _isDirty = true;
            }
        }

        public bool IsVaildFreeHotSpring() => this.dateTime_1.Date < DateTime.Now.Date;
    }
}