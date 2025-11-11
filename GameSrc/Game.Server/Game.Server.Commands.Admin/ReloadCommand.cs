using System.Linq;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base;

namespace Game.Server.Commands.Admin
{
	[Cmd("&load", ePrivLevel.Player, "Load the metedata.", new string[]
	{
		"   /load  [option]...  ",
		"Option:    /config     :Application config file.",
		"           /shop       :ShopMgr.ReLoad().",
		"           /item       :ItemMgr.Reload().",
		"           /property   :Game properties."
	})]
	public class ReloadCommand : AbstractCommandHandler, ICommandHandler
	{
		public bool OnCommand(BaseClient client, string[] args)
		{
			if (args.Length > 1)
			{
				StringBuilder builder = new StringBuilder();
				StringBuilder builder2 = new StringBuilder();
				if (args.Contains("/cmd"))
				{
					CommandMgr.LoadCommands();
					DisplayMessage(client, "Command load success!");
					builder.Append("/cmd,");
				}
				if (args.Contains("/config"))
				{
					GameServer.Instance.Configuration.Refresh();
					DisplayMessage(client, "Application config file load success!");
					builder.Append("/config,");
				}
				if (args.Contains("/property"))
				{
					GameProperties.Refresh();
					DisplayMessage(client, "Game properties load success!");
					builder.Append("/property,");
				}
				if (args.Contains("/item"))
				{
					if (ItemMgr.ReLoad())
					{
						DisplayMessage(client, "Items load success!");
						builder.Append("/item,");
					}
					else
					{
						DisplayMessage(client, "Items load failed!");
						builder2.Append("/item,");
					}
				}
				if (args.Contains("/shop"))
				{
					if (ItemMgr.ReLoad())
					{
						DisplayMessage(client, "Shops load success!");
						builder.Append("/shop,");
					}
					else
					{
						DisplayMessage(client, "Shops load failed!");
						builder2.Append("/shop,");
					}
				}
				if (builder.Length == 0 && builder2.Length == 0)
				{
					DisplayMessage(client, "Nothing executed!");
					DisplaySyntax(client);
				}
				else
				{
					DisplayMessage(client, "Success Options:    " + builder.ToString());
					if (builder2.Length > 0)
					{
						DisplayMessage(client, "Faile Options:      " + builder2.ToString());
						return false;
					}
				}
				return true;
			}
			DisplaySyntax(client);
			return true;
		}
	}
}
