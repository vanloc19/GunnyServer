using System;

namespace Game.Server.DDTQiYuan
{
    public class QiYuanProcessorAtribute : Attribute
    {
        private byte _code;

        private string _descript;

        public QiYuanProcessorAtribute(byte code, string description)
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