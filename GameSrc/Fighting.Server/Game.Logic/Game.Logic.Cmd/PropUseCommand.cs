using System.Collections.Generic;
using System.Linq;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Cmd
{
	[GameCommand(32, "使用道具")]
	public class PropUseCommand : ICommandHandler
	{
		public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
		{
			if (game.GameState != eGameState.Playing || player.GetSealState())
			{
				return;
			}
			int bag = packet.ReadByte();
			int place = packet.ReadInt();
			int templateId = packet.ReadInt();
			ItemTemplateInfo item = ItemMgr.FindItemTemplate(templateId);
			int[] propBag = PropItemMgr.PropBag;
			if ((bag == 2 && !new List<string>
			{
				"10001",
				"10002",
				"10003",
				"10004",
				"10005",
				"10006",
				"10007",
				"10008",
				"10009",
				"10010",
				"10011",
				"10012",
				"10013",
				"10014",
				"10015",
				"10016",
				"10017",
				"10018",
				"10019",
				"10020",
				"10021",
				"10022"
			}.Contains(item.TemplateID.ToString())) || item == null || !player.CanUseItem(item))
			{
				return;
			}
			if (templateId == 10001 || templateId == 10002 || templateId == 10003)
			{
				player.CanFly = false;
			}
			if (player.PlayerDetail.UsePropItem(game, bag, place, templateId, player.IsLiving))
			{
				if (!player.UseItem(item))
				{
					BaseGame.log.Error("Using prop error");
				}
			}
			else
			{
				if (bag == 2 && !player.PlayerDetail.UsePropItem(game, bag, place, templateId, player.IsLiving))
				{
					return;
				}
				if (propBag.Contains(item.TemplateID))
				{
					//fix sinh linh
					player.UseItem(item,place);
					switch (templateId)
					{
						case 10001:
							player.Prop1++;
							if (player.Prop < templateId * 2)
							{
								player.Prop += templateId;
								return;
							}
							break;
						case 10003:
							player.Prop2++;
							return;
						default:
							return;
						case 10004:
							if (player.Prop < templateId * 2)
							{
								player.Prop += templateId;
								return;
							}
							break;
					}
				}
				else
				{
					player.PlayerDetail.SendMessage("O item não pode ser usado");
				}
				if (templateId == 10015 || item.CategoryID == 17)
				{
					player.ShootCount = 1;
				}
			}
		}
	}
}
