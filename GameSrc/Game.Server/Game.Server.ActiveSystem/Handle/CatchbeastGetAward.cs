using System.Collections.Generic;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.ActiveSystem.Handle
{
    [ActiveSystemHandleAttbute((byte)ActiveSystemPackageType.CATCHBEAST_GETAWARD)]
    public class CatchbeastGetAward : IActiveSystemCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            GSPacketIn pkg = new GSPacketIn(145, Player.PlayerCharacter.ID);

            int id = packet.ReadInt();
            string[] YearMonsterBoxInfo = GameProperties.YearMonsterBoxInfo.Split('|');
            bool isGet = CanGetGift(Player.Actives.Info.DamageNum, id, YearMonsterBoxInfo);
            if (isGet)
            {
                int templateID = int.Parse(YearMonsterBoxInfo[id].Split(',')[0]);
                pkg.WriteByte(36);
                pkg.WriteBoolean(isGet); //var _loc_3:* = _loc_2.readBoolean();
                pkg.WriteInt(id); //var _loc_4:* = _loc_2.readInt();
                Player.Out.SendTCP(pkg);
                Player.Actives.SetYearMonterBoxState(id);
                List<ItemInfo> infos = new List<ItemInfo>();
                int num4 = 0;
                int num5 = 0;
                int num6 = 0;
                int num7 = 0;
                int num8 = 0;
                int num10 = 0;
                int num11 = 0;
                int num12 = 0;
                int num13 = 0;
                ItemBoxMgr.CreateItemBox(templateID, infos, ref num4, ref num5, ref num6, ref num7, ref num8, ref num10, ref num11, ref num12, ref num13);
                StringBuilder msga = new StringBuilder();
                foreach (ItemInfo item in infos)
                {
                    msga.Append(item.Template.Name + " x" + item.Count.ToString() + ", ");
                }

                Player.Out.SendMessage(eMessageType.Normal, msga.ToString());
                Player.AddTemplate(infos);
            }

            return true;
        }

        private bool CanGetGift(int damageNum, int id, string[] YearMonsterBoxInfo)
        {
            if (id > 4 || id < 0)
                return false;
            int box = int.Parse(YearMonsterBoxInfo[id].Split(',')[1]) * 10000;
            return box <= damageNum;
        }
    }
}