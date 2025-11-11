using System;

namespace SqlDataProvider.Data
{
    public class DebrisInfo
    {
        private int _ID, _chapterID, _pageID;
        private DateTime _date;
        private bool _isDirty;

        public bool IsDirty
        {
            get => _isDirty;
            set => _isDirty = value;
        }

        [SqlField]
        public int ID
        {
            get => _ID;
            set
            {
                _isDirty = true;
                _ID = value;
            }
        }

        [SqlField]
        public int chapterID
        {
            get => _chapterID;
            set
            {
                _isDirty = true;
                _chapterID = value;
            }
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
        public DateTime date
        {
            get => _date;
            set
            {
                _isDirty = true;
                _date = value;
            }
        }
    }
}