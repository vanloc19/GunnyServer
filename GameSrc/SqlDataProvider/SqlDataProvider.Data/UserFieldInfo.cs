using System;

namespace SqlDataProvider.Data
{
    public class UserFieldInfo : DataObject
    {
        private int _id;

        public int ID
        {
            get { return _id; }
            set
            {
                _id = value;
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

        private int _fieldID;

        public int FieldID
        {
            get { return _fieldID; }
            set
            {
                _fieldID = value;
                _isDirty = true;
            }
        }

        private int _seedID;

        public int SeedID
        {
            get { return _seedID; }
            set
            {
                _seedID = value;
                _isDirty = true;
            }
        }

        private DateTime _plantTime;

        public DateTime PlantTime
        {
            get { return _plantTime; }
            set
            {
                _plantTime = value;
                _isDirty = true;
            }
        }

        private int _accelerateTime;

        public int AccelerateTime
        {
            get { return _accelerateTime; }
            set
            {
                _accelerateTime = value;
                _isDirty = true;
            }
        }

        private int _fieldValidDate;

        public int FieldValidDate
        {
            get { return _fieldValidDate; }
            set
            {
                _fieldValidDate = value;
                _isDirty = true;
            }
        }

        private DateTime _payTime;

        public DateTime PayTime
        {
            get { return _payTime; }
            set
            {
                _payTime = value;
                _isDirty = true;
            }
        }

        //payFieldTime
        private int _payFieldTime;

        public int payFieldTime
        {
            get { return _payFieldTime; }
            set
            {
                _payFieldTime = value;
                _isDirty = true;
            }
        }

        public bool IsValidField()
        {
            if (_payFieldTime != 0)
            {
                return DateTime.Compare(_payTime.AddDays(_payFieldTime), DateTime.Now) > 0;
            }

            return true;
        }

        private int _gainCount;

        public int GainCount
        {
            get { return _gainCount; }
            set
            {
                _gainCount = value;
                _isDirty = true;
            }
        }

        private int _gainFieldId;

        public int gainFieldId
        {
            get { return _gainFieldId; }
            set
            {
                _gainFieldId = value;
                _isDirty = true;
            }
        }

        private int _autoSeedID;

        public int AutoSeedID
        {
            get { return _autoSeedID; }
            set
            {
                _autoSeedID = value;
                _isDirty = true;
            }
        }

        private int _autoFertilizerID;

        public int AutoFertilizerID
        {
            get { return _autoFertilizerID; }
            set
            {
                _autoFertilizerID = value;
                _isDirty = true;
            }
        }

        private int _autoSeedIDCount;

        public int AutoSeedIDCount
        {
            get { return _autoSeedIDCount; }
            set
            {
                _autoSeedIDCount = value;
                _isDirty = true;
            }
        }

        private int _autoFertilizerCount;

        public int AutoFertilizerCount
        {
            get { return _autoFertilizerCount; }
            set
            {
                _autoFertilizerCount = value;
                _isDirty = true;
            }
        }

        private bool _isAutomatic;

        public bool isAutomatic
        {
            get { return _isAutomatic; }
            set
            {
                _isAutomatic = value;
                _isDirty = true;
            }
        }

        private bool _isExist;

        public bool IsExit
        {
            get { return _isExist; }
            set
            {
                _isExist = value;
                _isDirty = true;
            }
        }

        private DateTime _automaticTime;

        public DateTime AutomaticTime
        {
            get { return _automaticTime; }
            set
            {
                _automaticTime = value;
                _isDirty = true;
            }
        }

        public bool isDig()
        {
            TimeSpan usedTime = DateTime.Now - _plantTime;
            int timeLeft = _fieldValidDate - ((int)usedTime.TotalMinutes + _accelerateTime);
            return timeLeft <= 0;
        }
    }
}