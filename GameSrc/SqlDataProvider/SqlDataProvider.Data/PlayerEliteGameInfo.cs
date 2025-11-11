using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlDataProvider.Data
{
    public class PlayerEliteGameInfo
    {
        public int UserID { get; set; }

        private string _NickName;
        public int GroupType { get; set; }
        public string NickName
        {
            get { return _NickName; }
            set { _NickName = value; }
        }


        public int GameType { get; set; }

        public int Rank { get; set; }

        public int CurrentPoint { get; set; }
        public int Status { get; set; }

        private int _order;
        public int MatchOrderNumber
        {
            get { return _order; }
            set
            {
                _order = value;
            }
        }
        public int Winer { get; set; }

        public bool ReadyStatus { get; set; }

        public int AreaId { get; set; }

        public int RoundType { get; set; }

        public int Grade { get; set; }

        public int Blood { get; set; }

        public int FightPower { get; set; }

        public int UserMoneyPay { get; set; }

        public int TotalMatch { get; set; }//open dbo.Elite_Game_Data as Design add TotalMatch set DefaultValue or Bindding  = 0

        public bool CheckedWon { get; set; }
    }
}
