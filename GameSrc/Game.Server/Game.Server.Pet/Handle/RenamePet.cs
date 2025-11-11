using System;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Pet.Handle
{
    [global::Pet(9)]
    public class RenamePet : IPetCommandHadler
    {
        public bool CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            int place = packet.ReadInt();
            string name = packet.ReadString();
            int changeNameCost = Convert.ToInt32(PetMgr.FindConfig("ChangeNameCost").Value);
            if (player.MoneyDirect(changeNameCost,IsAntiMult:false))
            {
                if (player.PetBag.RenamePet(place, name))
                {
                    player.SendMessage(LanguageMgr.GetTranslation("PetHandler.Msg20"));
                }
            }

            return false;
        }
    }
}