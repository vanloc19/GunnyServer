using System;

namespace SqlDataProvider.Data
{
    public class AchievementData : DataObject
    {
        private bool _IsComplete;
        private DateTime _CompleteDate;
        private int _UserID;
        private int _AchievementID;

        public AchievementData()
        {
            this.UserID = 0;
            this.AchievementID = 0;
            this.IsComplete = false;
            this.CompletedDate = DateTime.Now;
        }

        public int AchievementID
        {
            get => this._AchievementID;
            set
            {
                this._AchievementID = value;
                this._isDirty = true;
            }
        }

        public DateTime CompletedDate
        {
            get => this._CompleteDate;
            set
            {
                this._CompleteDate = value;
                this._isDirty = true;
            }
        }

        public bool IsComplete
        {
            get => this._IsComplete;
            set
            {
                this._IsComplete = value;
                this._isDirty = true;
            }
        }

        public int UserID
        {
            get => this._UserID;
            set
            {
                this._UserID = value;
                this._isDirty = true;
            }
        }
    }
}