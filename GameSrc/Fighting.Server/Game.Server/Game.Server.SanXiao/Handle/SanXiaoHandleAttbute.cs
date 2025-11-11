using System;

namespace Game.Server.SanXiao.Handle
{
    class SanXiaoHandleAttbute : Attribute
    {
        public byte Code { get; private set; }

        public SanXiaoHandleAttbute(byte code)
        {
            Code = code;
        }
    }
}