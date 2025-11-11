using System;

namespace Game.Server.AvatarCollection.Handle
{
    class AvatarCollectionHandleAttbute : Attribute
    {
        public byte Code { get; private set; }

        public AvatarCollectionHandleAttbute(byte code)
        {
            Code = code;
        }
    }
}