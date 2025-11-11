namespace SqlDataProvider.Data
{
    public class PagesInfo
    {
        private int _pageID;
        private bool _activate;
        private bool _isDirty;

        public bool IsDirty
        {
            get => _isDirty;
            set => _isDirty = value;
        }

        [SqlField]
        public int pageID
        {
            get => _pageID;
            set
            {
                _isDirty = true;
                _pageID = value;
            }
        }

        [SqlField]
        public bool activate
        {
            get => _activate;
            set
            {
                _isDirty = true;
                _activate = value;
            }
        }
    }
}