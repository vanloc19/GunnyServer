using System;

namespace Game.Server.ExplorerManual.Handle
{
    class ExplorerManualHandleAttbute : Attribute
    {
        public byte Code { get; private set; }

        public ExplorerManualHandleAttbute(byte code)
        {
            Code = code;
        }
    }
}