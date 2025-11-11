using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Managers
{
	public class FusionCombined
	{
		public static Items_Fusion_List_Info[] m_itemsfusionlist;

		public static FusionCombinedInfo[] m_listFusionCombined;

		public static FusionCombinedInfo[] ListCombinedFusion()
		{
			using (ProduceBussiness bussiness = new ProduceBussiness())
			{
				m_itemsfusionlist = bussiness.GetAllFusionList();
			}
			return m_listFusionCombined;
		}
	}
}
