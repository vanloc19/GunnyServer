using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Bussiness;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using log4net;
using SqlDataProvider.Data;

namespace Game.Logic
{
	public class BallMgr
	{
		private static Dictionary<int, BallInfo> dictionary_0;

		private static Dictionary<int, Tile> dictionary_1;

		private static readonly ILog ilog_0;

		static BallMgr()
		{
			ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		public static BallInfo FindBall(int id)
		{
			if (dictionary_0.ContainsKey(id))
			{
				return dictionary_0[id];
			}
			return null;
		}

		public static Tile FindTile(int id)
		{
			if (dictionary_1.ContainsKey(id))
			{
				return dictionary_1[id];
			}
			return null;
		}

		public static BombType GetBallType(int ballId)
		{
			switch(ballId)
            {
				case 1:
				case 56:
				case 99:
					return BombType.FORZEN;
				case 3:
					return BombType.FLY;
				case 5:
				case 64:
				case 59:
				case 97:
				case 98:
				case 120:
				case 10009:
					return BombType.CURE;
				case 110:
				case 117:
					return BombType.WORLDCUP;
				case 128:
				case 129:
					return BombType.CATCHINSECT;
				default:
					return BombType.Normal;

			}
            #region OLD BallType
            //if (ballId <= 59)
            //{
            //	switch (ballId)
            //	{
            //	case 2:
            //	case 4:
            //		return BombType.Normal;
            //	case 3:
            //		return BombType.FLY;
            //	case 5:
            //		return BombType.CURE;
            //	default:
            //		return BombType.Normal;
            //	case 59:
            //		return BombType.CURE;
            //	case 1:
            //	case 56:
            //		break;
            //	}
            //}
            //else
            //{
            //	if (ballId > 99)
            //	{
            //		if (ballId == 120 || ballId == 10009)
            //		{
            //			return BombType.CURE;
            //		}
            //		return BombType.Normal;
            //	}
            //	switch (ballId)
            //	{
            //	case 64:
            //		return BombType.CURE;
            //	case 97:
            //	case 98:
            //		return BombType.CURE;
            //	default:
            //		return BombType.Normal;
            //	case 99:
            //		break;
            //	}
            //}
            //return BombType.FORZEN;
            #endregion
        }

        public static bool Init()
		{
			return ReLoad();
		}

		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, BallInfo> dictionary = smethod_0();
				Dictionary<int, Tile> dictionary2 = smethod_1(dictionary);
				if (dictionary.Values.Count > 0 && dictionary2.Values.Count > 0)
				{
					Interlocked.Exchange(ref dictionary_0, dictionary);
					Interlocked.Exchange(ref dictionary_1, dictionary2);
					return true;
				}
			}
			catch (Exception exception)
			{
				ilog_0.Error("Ball Mgr init error:", exception);
			}
			return false;
		}

		private static Dictionary<int, BallInfo> smethod_0()
		{
			Dictionary<int, BallInfo> dictionary = new Dictionary<int, BallInfo>();
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				BallInfo[] allBall = produceBussiness.GetAllBall();
				foreach (BallInfo ballInfo in allBall)
				{
					if (!dictionary.ContainsKey(ballInfo.ID))
					{
						dictionary.Add(ballInfo.ID, ballInfo);
					}
				}
				return dictionary;
			}
		}

		private static Dictionary<int, Tile> smethod_1(Dictionary<int, BallInfo> list)
		{
			Dictionary<int, Tile> dictionary = new Dictionary<int, Tile>();
			foreach (BallInfo current in list.Values)
			{
				if (current.HasTunnel)
				{
					string text = $"bomb\\{current.ID}.bomb";
					Tile tile = null;
					if (File.Exists(text))
					{
						tile = new Tile(text, digable: false);
					}
					dictionary.Add(current.ID, tile);
					if (tile == null && current.ID != 1 && current.ID != 2 && current.ID != 3)
					{
						ilog_0.ErrorFormat("can't find bomb file:{0}", text);
					}
				}
			}
			return dictionary;
		}
	}
}
