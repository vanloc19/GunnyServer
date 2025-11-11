using System;

namespace Game.Server.DDTQiYuan.Handle
{
    public class QiYuanHandleAttbute : Attribute
    {
        public byte Code { get; private set; }

        public QiYuanHandleAttbute(byte code)
        {
            Code = code;
        }
    }
}