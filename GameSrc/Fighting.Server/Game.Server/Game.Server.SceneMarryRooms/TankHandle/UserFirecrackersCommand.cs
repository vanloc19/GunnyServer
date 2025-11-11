using System.Linq;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.SceneMarryRooms.TankHandle
{
	[MarryCommandAttbute(6)]
	public class UserFirecrackersCommand : IMarryCommandHandler
	{
		public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
		{
			if (player.CurrentMarryRoom != null)
			{
				packet.ReadInt();
				ShopItemInfo info = ShopMgr.FindShopbyTemplatID(packet.ReadInt()).FirstOrDefault();
				if (info != null)
				{
					if (info.APrice1 == -2)
					{
						if (player.PlayerCharacter.Gold >= info.AValue1)
						{
							player.RemoveGold(info.AValue1);
							player.CurrentMarryRoom.ReturnPacketForScene(player, packet);
							player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("UserFirecrackersCommand.Successed1", info.AValue1));
							player.OnUsingItem(info.TemplateID, 1);
							return true;
						}
						player.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, LanguageMgr.GetTranslation("UserFirecrackersCommand.GoldNotEnough"));
					}
					if (info.APrice1 == -1)
					{
						if (player.PlayerCharacter.Money + player.PlayerCharacter.MoneyLock >= info.AValue1)
						{
							player.RemoveMoney(info.AValue1, isConsume: false);
							player.CurrentMarryRoom.ReturnPacketForScene(player, packet);
							player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("UserFirecrackersCommand.Successed2", info.AValue1));
							player.OnUsingItem(info.TemplateID, 1);
							return true;
						}
						player.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("UserFirecrackersCommand.MoneyNotEnough"));
					}
				}
			}
			return false;
		}
	}
}
