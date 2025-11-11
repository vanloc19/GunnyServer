namespace SqlDataProvider.Data
{
    public class EatPetsInfo : DataObject
    {
        private int _iD;

        public int ID
        {
            get { return _iD; }
            set
            {
                _iD = value;
                _isDirty = true;
            }
        }

        private int _userID;

        public int UserID
        {
            get { return _userID; }
            set
            {
                _userID = value;
                _isDirty = true;
            }
        }

        private int _weaponExp;

        public int weaponExp
        {
            get { return _weaponExp; }
            set
            {
                _weaponExp = value;
                _isDirty = true;
            }
        }

        private int _weaponLevel;

        public int weaponLevel
        {
            get { return _weaponLevel; }
            set
            {
                _weaponLevel = value;
                _isDirty = true;
            }
        }

        private int _clothesExp;

        public int clothesExp
        {
            get { return _clothesExp; }
            set
            {
                _clothesExp = value;
                _isDirty = true;
            }
        }

        private int _clothesLevel;

        public int clothesLevel
        {
            get { return _clothesLevel; }
            set
            {
                _clothesLevel = value;
                _isDirty = true;
            }
        }

        private int _hatExp;

        public int hatExp
        {
            get { return _hatExp; }
            set
            {
                _hatExp = value;
                _isDirty = true;
            }
        }

        private int _hatLevel;

        public int hatLevel
        {
            get { return _hatLevel; }
            set
            {
                _hatLevel = value;
                _isDirty = true;
            }
        }
    }
}