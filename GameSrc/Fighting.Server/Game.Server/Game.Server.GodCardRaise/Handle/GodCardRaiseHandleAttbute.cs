using System;

namespace Game.Server.GodCardRaise.Handle
{
    class GodCardRaiseHandleAttbute : Attribute
    {
        public byte Code { get; private set; }

        public GodCardRaiseHandleAttbute(byte code)
        {
            Code = code;
        }
    }
}