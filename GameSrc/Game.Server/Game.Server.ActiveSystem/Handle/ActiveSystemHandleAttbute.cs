using System;

namespace Game.Server.ActiveSystem.Handle
{
    class ActiveSystemHandleAttbute : Attribute
    {
        public byte Code { get; private set; }

        public ActiveSystemHandleAttbute(byte code)
        {
            Code = code;
        }
    }
}