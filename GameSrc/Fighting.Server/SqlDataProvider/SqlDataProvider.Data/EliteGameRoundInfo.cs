using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlDataProvider.Data
{
    public class EliteGameRoundInfo
    {
        private int _RoundID;

        public int RoundID
        {
            get { return _RoundID; }
            set { _RoundID = value; }
        }

        private int _RoundType;

        public int RoundType
        {
            get { return _RoundType; }
            set { _RoundType = value; }
        }


        private PlayerEliteGameInfo _PlayerOne;

        public PlayerEliteGameInfo PlayerOne
        {
            get { return _PlayerOne; }
            set { _PlayerOne = value; }
        }

        private PlayerEliteGameInfo _PlayerTwo;

        public PlayerEliteGameInfo PlayerTwo
        {
            get { return _PlayerTwo; }
            set { _PlayerTwo = value; }
        }

        private PlayerEliteGameInfo _PlayerWin;

        public PlayerEliteGameInfo PlayerWin
        {
            get { return _PlayerWin; }
            set { _PlayerWin = value; }
        }

    }
}
