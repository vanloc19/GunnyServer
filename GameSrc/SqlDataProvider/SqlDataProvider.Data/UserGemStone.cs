namespace SqlDataProvider.Data
{
    public class UserGemStone : DataObject
    {
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

        private int _figSpiritId;

        public int FigSpiritId
        {
            get { return _figSpiritId; }
            set
            {
                _figSpiritId = value;
                _isDirty = true;
            }
        }

        private string _figSpiritIdValue;

        public string FigSpiritIdValue
        {
            get { return _figSpiritIdValue; }
            set
            {
                _figSpiritIdValue = value;
                _isDirty = true;
            }
        }

        private int _equipPlace;

        public int EquipPlace
        {
            get { return _equipPlace; }
            set
            {
                _equipPlace = value;
                _isDirty = true;
            }
        }
    }
}