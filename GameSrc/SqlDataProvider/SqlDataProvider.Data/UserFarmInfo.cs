using System;

namespace SqlDataProvider.Data
{
    public class UserFarmInfo : DataObject
    {
        private UserFieldInfo _field;

        public UserFieldInfo Field
        {
            get { return _field; }
            set
            {
                _field = value;
                _isDirty = true;
            }
        }

        private int _ID;

        public int ID
        {
            get { return _ID; }
            set
            {
                _ID = value;
                _isDirty = true;
            }
        }

        private int _farmID;

        public int FarmID
        {
            get { return _farmID; }
            set
            {
                _farmID = value;
                _isDirty = true;
            }
        }

        private string _payFieldMoney;

        public string PayFieldMoney
        {
            get { return _payFieldMoney; }
            set
            {
                _payFieldMoney = value;
                _isDirty = true;
            }
        }

        private string _payAutoMoney;

        public string PayAutoMoney
        {
            get { return _payAutoMoney; }
            set
            {
                _payAutoMoney = value;
                _isDirty = true;
            }
        }

        private DateTime _autoPayTime;

        public DateTime AutoPayTime
        {
            get { return _autoPayTime; }
            set
            {
                _autoPayTime = value;
                _isDirty = true;
            }
        }

        private int _autoValidDate;

        public int AutoValidDate
        {
            get { return _autoValidDate; }
            set
            {
                _autoValidDate = value;
                _isDirty = true;
            }
        }

        private int _vipLimitLevel;

        public int VipLimitLevel
        {
            get { return _vipLimitLevel; }
            set
            {
                _vipLimitLevel = value;
                _isDirty = true;
            }
        }

        private string _farmerName;

        public string FarmerName
        {
            get { return _farmerName; }
            set
            {
                _farmerName = value;
                _isDirty = true;
            }
        }

        private int _gainFieldId;

        public int GainFieldId
        {
            get { return _gainFieldId; }
            set
            {
                _gainFieldId = value;
                _isDirty = true;
            }
        }

        private int _matureId;

        public int MatureId
        {
            get { return _matureId; }
            set
            {
                _matureId = value;
                _isDirty = true;
            }
        }

        private int _killCropId;

        public int KillCropId
        {
            get { return _killCropId; }
            set
            {
                _killCropId = value;
                _isDirty = true;
            }
        }

        private int _isAutoId;

        public int isAutoId
        {
            get { return _isAutoId; }
            set
            {
                _isAutoId = value;
                _isDirty = true;
            }
        }

        private bool _isFarmHelper;

        public bool isFarmHelper
        {
            get { return _isFarmHelper; }
            set
            {
                _isFarmHelper = value;
                _isDirty = true;
            }
        }

        private int _buyExpRemainNum;

        public int buyExpRemainNum
        {
            get { return _buyExpRemainNum; }
            set
            {
                _buyExpRemainNum = value;
                _isDirty = true;
            }
        }

        private bool _isArrange;

        public bool isArrange
        {
            get { return _isArrange; }
            set
            {
                _isArrange = value;
                _isDirty = true;
            }
        }

        private int _treeLevel;

        public int TreeLevel
        {
            get { return _treeLevel; }
            set
            {
                _treeLevel = value;
                _isDirty = true;
            }
        }

        private int _treeExp;

        public int TreeExp
        {
            get { return _treeExp; }
            set
            {
                _treeExp = value;
                _isDirty = true;
            }
        }

        private int _loveScore;

        public int LoveScore
        {
            get { return _loveScore; }
            set
            {
                _loveScore = value;
                _isDirty = true;
            }
        }

        private int _monsterExp;

        public int MonsterExp
        {
            get { return _monsterExp; }
            set
            {
                _monsterExp = value;
                _isDirty = true;
            }
        }

        private int _poultryState;

        public int PoultryState
        {
            get { return _poultryState; }
            set
            {
                _poultryState = value;
                _isDirty = true;
            }
        }

        private DateTime _countDownTime;

        public DateTime CountDownTime
        {
            get { return _countDownTime; }
            set
            {
                _countDownTime = value;
                _isDirty = true;
            }
        }

        public bool isFeed()
        {
            TimeSpan usedTime = DateTime.Now - _countDownTime;
            int timeLeft = 60 * 60 * 1000 - (int)usedTime.TotalMilliseconds;
            timeLeft = timeLeft / 60 / 60 / 1000;
            return timeLeft <= 0;
        }

        private int _treeCostExp;

        public int TreeCostExp
        {
            get { return _treeCostExp; }
            set
            {
                _treeCostExp = value;
                _isDirty = true;
            }
        }
    }
}