using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.ActiveSystem.Handle
{
    [ActiveSystemHandleAttbute((byte)ActiveSystemPackageType.PYRAMID_ENTER)]
    public class PyramidEnter : IActiveSystemCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            if (Player.Actives.LoadPyramid())
            {
                GSPacketIn pkg = new GSPacketIn(145, Player.PlayerCharacter.ID);
                PyramidInfo pyramid = Player.Actives.Pyramid;
                pkg.WriteByte((byte)ActiveSystemPackageType.PYRAMID_ENTER);
                pkg.WriteInt(pyramid.currentLayer); //this.model.currentLayer = param1.readInt();
                pkg.WriteInt(pyramid.maxLayer); //this.model.maxLayer = param1.readInt();
                pkg.WriteInt(pyramid.totalPoint); //this.model.totalPoint = param1.readInt();
                pkg.WriteInt(pyramid.turnPoint); //this.model.turnPoint = param1.readInt();
                pkg.WriteInt(pyramid.pointRatio); //this.model.pointRatio = param1.readInt();
                pkg.WriteInt(pyramid.currentFreeCount); //this.model.currentFreeCount = param1.readInt();
                pkg.WriteInt(pyramid.currentReviveCount); //this.model.currentReviveCount = param1.readInt();
                pkg.WriteBoolean(pyramid.isPyramidStart); //this.model.isPyramidStart = param1.readBoolean();
                if (pyramid.isPyramidStart)
                {
                    string[] listsLayerItems = pyramid.LayerItems.Split('|');
                    int[] layer = new int[] { 8, 7, 6, 5, 4, 3, 2 };
                    pkg.WriteInt(layer.Length); //_loc_2 = param1.readInt();
                    int mainKey = 1;
                    while (mainKey <= layer.Length)
                    {
                        string[] listTemplaleId = GetLayerItems(listsLayerItems, mainKey);
                        pkg.WriteInt(mainKey); //_loc_4 = param1.readInt();                                    
                        pkg.WriteInt(listTemplaleId.Length); //_loc_5 = param1.readInt();
                        int subKey = 0;
                        while (subKey < listTemplaleId.Length)
                        {
                            int templateId = int.Parse(listTemplaleId[subKey].Split('-')[1]);
                            int place = int.Parse(listTemplaleId[subKey].Split('-')[2]);
                            pkg.WriteInt(templateId); //_loc_7 = param1.readInt();TemplateID
                            pkg.WriteInt(place); //_loc_8 = param1.readInt();Place
                            subKey++;
                        }

                        mainKey++;
                    }
                }

                Player.SendTCP(pkg);
            }
            else
            {
                Player.SendMessage(LanguageMgr.GetTranslation("ActiveSystemHandler.Msg10"));
            }

            return true;
        }

        private string[] GetLayerItems(string[] lists, int layer)
        {
            List<string> listStr = new List<string>();

            foreach (string str in lists)
            {
                string tempLayer = str.Split('-')[0];
                if (tempLayer == layer.ToString())
                {
                    listStr.Add(str);
                }
            }

            return listStr.ToArray();
        }
    }
}