using System;

namespace Game.Server.ActiveSystem
{
    public class ActiveSystemProcessorAtribute : Attribute
    {
        private byte _code;

        private string _descript;

        public ActiveSystemProcessorAtribute(byte code, string description)
        {
            _code = code;

            _descript = description;
        }

        public byte Code
        {
            get { return _code; }
        }

        public string Description
        {
            get { return _descript; }
        }
    }
}