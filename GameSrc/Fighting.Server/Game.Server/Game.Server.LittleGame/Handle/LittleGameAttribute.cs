using System;

namespace Game.Server.LittleGame.Handle
{
    internal class LittleGameAttribute : Attribute
    {
        public LittleGameAttribute(byte byteCode)
        {
            this.byteCode = byteCode;
        }
        private byte byteCode;

        public byte GetByteCode()
        {
            return this.byteCode;
        }

    }
}