//using System.Collections.Generic;
//using System.Drawing;
//using Game.Logic.Phy.Object;

//namespace Game.Logic.Actions
//{
//	public class LivingMoveToAction : BaseAction
//	{
//		private string m_action;

//		private LivingCallBack m_callback;

//		private int m_delayCallback;

//		private int m_index;

//		private bool m_isSent;

//		private Living m_living;

//		private List<Point> m_path;

//		private string m_saction;

//		private int m_speed;

//		public LivingMoveToAction(Living living, List<Point> path, string action, int delay, int speed, LivingCallBack callback)
//			: base(delay, 0)
//		{
//			m_living = living;
//			m_path = path;
//			m_action = action;
//			m_isSent = false;
//			m_index = 0;
//			m_callback = callback;
//			m_speed = speed;
//		}

//		public LivingMoveToAction(Living living, List<Point> path, string action, int delay, int speed, string sAction, LivingCallBack callback, int delayCallback)
//			: base(delay, 0)
//		{
//			m_living = living;
//			m_path = path;
//			m_action = action;
//			m_saction = sAction;
//			m_isSent = false;
//			m_index = 0;
//			m_callback = callback;
//			m_speed = speed;
//			m_delayCallback = delayCallback;
//		}

//		protected override void ExecuteImp(BaseGame game, long tick)
//		{
//			if (!m_isSent)
//			{
//				m_isSent = true;
//				Point point = m_path[m_path.Count - 1];
//				point = m_path[m_path.Count - 1];
//				game.SendLivingMoveTo(m_living, m_living.X, m_living.Y, point.X, point.Y, m_action, m_speed, m_saction);
//			}
//			m_index++;
//			if (m_index >= m_path.Count)
//			{
//				if (m_path[m_index - 1].X > m_living.X)
//				{
//					m_living.Direction = 1;
//				}
//				else
//				{
//					m_living.Direction = -1;
//				}
//				Point point = m_path[m_index - 1];
//				point = m_path[m_index - 1];
//				m_living.SetXY(point.X, point.Y);
//				if (m_callback != null)
//				{
//					m_living.CallFuction(m_callback, 0);
//				}
//				Finish(tick);
//			}
//		}
//	}
//}

using Game.Logic.Phy.Object;
using System.Collections.Generic;
using System.Drawing;

namespace Game.Logic.Actions
{
	public class LivingMoveToAction : BaseAction
	{
		private Living living_0;

		private List<Point> list_0;

		private string string_0;

		private string string_1;

		private bool bool_0;

		private int int_0;

		private int int_1;

		private LivingCallBack livingCallBack_0;

		private int int_2;

		public LivingMoveToAction(Living living, List<Point> path, string action, string saction, int speed, int delay, LivingCallBack callback, int delayCallback) : base(delay, 0)
		{


			living_0 = living;
			list_0 = path;
			string_0 = action;
			string_1 = saction;
			bool_0 = false;
			int_0 = 0;
			livingCallBack_0 = callback;
			int_1 = speed;
			int_2 = delayCallback;
		}

		protected override void ExecuteImp(BaseGame game, long tick)
		{
			if (!bool_0)
			{
				bool_0 = true;
				game.method_26(living_0, living_0.X, living_0.Y, list_0[list_0.Count - 1].X, list_0[list_0.Count - 1].Y, string_0, string_1, int_1);
			}
			int_0++;
			if (int_0 >= list_0.Count)
			{
				if (list_0[int_0 - 1].X > living_0.X)
				{
					living_0.Direction = 1;
				}
				else
				{
					living_0.Direction = -1;
				}
				living_0.SetXY(list_0[int_0 - 1].X, list_0[int_0 - 1].Y);
				if (livingCallBack_0 != null)
				{
					living_0.CallFuction(livingCallBack_0, int_2);
				}
				Finish(tick);
			}
		}
	}
}

