using System;
using System.Collections;
using System.Reflection;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.RingStation.Action;
using Game.Server.RingStation.RoomGamePkg;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.RingStation
{
	public class RingStationGamePlayer
	{
		private bool _canUserProp = true;

		private int _ID;

		public BaseRoomRingStation CurRoom;

		public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private ArrayList m_actions = new ArrayList();

		private long m_passTick;

		private RoomGame seflRoom = new RoomGame();

		public int Agility
		{
			get;
			set;
		}

		public double AntiAddictionRate
		{
			get;
			set;
		}

		public int Attack
		{
			get;
			set;
		}

		public float AuncherExperienceRate
		{
			get;
			set;
		}

		public float AuncherOfferRate
		{
			get;
			set;
		}

		public float AuncherRichesRate
		{
			get;
			set;
		}

		public int badgeID
		{
			get;
			set;
		}

		public double BaseAgility
		{
			get;
			set;
		}

		public double BaseAttack
		{
			get;
			set;
		}

		public double BaseBlood
		{
			get;
			set;
		}

		public double BaseDefence
		{
			get;
			set;
		}

		public bool CanUserProp
		{
			get
			{
				return _canUserProp;
			}
			set
			{
				_canUserProp = value;
			}
		}

		public string Colors
		{
			get;
			set;
		}

		public int ConsortiaID
		{
			get;
			set;
		}

		public int ConsortiaLevel
		{
			get;
			set;
		}

		public string ConsortiaName
		{
			get;
			set;
		}

		public int ConsortiaRepute
		{
			get;
			set;
		}

		public int Dander
		{
			get;
			set;
		}

		public int Defence
		{
			get;
			set;
		}

		public int Direction
		{
			get;
			set;
		}

		public int FightPower
		{
			get;
			set;
		}

		public bool FirtDirection
		{
			get;
			set;
		}

		public int GamePlayerId
		{
			get
			{
				return _ID;
			}
			set
			{
				_ID = value;
			}
		}

		public float GMExperienceRate
		{
			get;
			set;
		}

		public float GMOfferRate
		{
			get;
			set;
		}

		public float GMRichesRate
		{
			get;
			set;
		}

		public int GP
		{
			get;
			set;
		}

		public double GPAddPlus
		{
			get;
			set;
		}

		public int Grade
		{
			get;
			set;
		}

		public int Healstone
		{
			get;
			set;
		}

		public int HealstoneCount
		{
			get;
			set;
		}

		public int Hide
		{
			get;
			set;
		}

		public string Honor
		{
			get;
			set;
		}

		public int hp
		{
			get;
			set;
		}

		public int ID
		{
			get
			{
				return _ID;
			}
			set
			{
				_ID = value;
			}
		}

		public int LastAngle
		{
			get;
			set;
		}

		public int LastForce
		{
			get;
			set;
		}

		public int LastX
		{
			get;
			set;
		}

		public int LastY
		{
			get;
			set;
		}

		public int Luck
		{
			get;
			set;
		}

		public int LX
		{
			get;
			set;
		}

		public int LY
		{
			get;
			set;
		}

		public string NickName
		{
			get;
			set;
		}

		public int Nimbus
		{
			get;
			set;
		}

		public int Offer
		{
			get;
			set;
		}

		public double OfferAddPlus
		{
			get;
			set;
		}

		public int Repute
		{
			get;
			set;
		}

		public int SecondWeapon
		{
			get;
			set;
		}

		public bool Sex
		{
			get;
			set;
		}

		public int ShootCount
		{
			get;
			set;
		}

		public string Skin
		{
			get;
			set;
		}

		public string Style
		{
			get;
			set;
		}

		public int StrengthLevel
		{
			get;
			set;
		}

		public int Team
		{
			get;
			set;
		}

		public int TemplateID
		{
			get;
			set;
		}

		public int Total
		{
			get;
			set;
		}

		public byte typeVIP
		{
			get;
			set;
		}

		public int VIPLevel
		{
			get;
			set;
		}

		public string WeaklessGuildProgressStr
		{
			get;
			set;
		}

		public int Win
		{
			get;
			set;
		}

		public int X
		{
			get;
			set;
		}

		public int Y
		{
			get;
			set;
		}

		public void AddAction(IAction action)
		{
			lock (m_actions)
			{
				m_actions.Add(action);
			}
		}

		public void AddAction(ArrayList action)
		{
			lock (m_actions)
			{
				m_actions.AddRange(action);
			}
		}

		public void AddTurn(GSPacketIn pkg)
		{
			if (pkg.Parameter1 == GamePlayerId)
			{
				m_actions.Add(new PlayerShotAction(LastX, LastY - 25, LastForce, LastAngle, 1000));
			}
		}

		public static double ComputeVx(double dx, float m, float af, float f, float t)
		{
			return (dx - (double)(f / m * t * t / 2f)) / (double)t + (double)(af / m) * dx * 0.8;
		}

		public static double ComputeVy(double dx, float m, float af, float f, float t)
		{
			return (dx - (double)(f / m * t * t / 2f)) / (double)t + (double)(af / m) * dx * 1.3;
		}

		public void FindTarget()
		{
			GSPacketIn pkg = new GSPacketIn(91)
			{
				Parameter1 = GamePlayerId
			};
			pkg.WriteByte(143);
			SendTCP(pkg);
		}

		public void NextTurn(GSPacketIn pkg)
		{
			SendSelfTurn(pkg.Parameter1 == GamePlayerId, useBuff: true);
		}

		public void Pause(int time)
		{
			m_passTick = Math.Max(m_passTick, TickHelper.GetTickCount() + time);
		}

		internal void ProcessPacket(GSPacketIn pkg)
		{
			if (seflRoom != null)
			{
				seflRoom.ProcessData(this, pkg);
			}
		}

		public void Resume()
		{
			m_passTick = 0L;
		}

		public void SendCreateGame(GSPacketIn pkg)
		{
			ShootCount = 100;
			FirtDirection = true;
			Direction = -1;
			pkg.ReadInt();
			pkg.ReadInt();
			pkg.ReadInt();
			int num = pkg.ReadInt();
			int num3 = 0;
			for (int i = 0; i < num; i++)
			{
				pkg.ReadInt();
				pkg.ReadString();
				pkg.ReadInt();
				pkg.ReadString();
				pkg.ReadBoolean();
				pkg.ReadByte();
				pkg.ReadInt();
				pkg.ReadBoolean();
				pkg.ReadInt();
				pkg.ReadString();
				pkg.ReadString();
				pkg.ReadString();
				pkg.ReadInt();
				pkg.ReadInt();
				if (pkg.ReadInt() != 0)
				{
					pkg.ReadInt();
					pkg.ReadString();
					pkg.ReadDateTime();
				}
				pkg.ReadInt();
				pkg.ReadInt();
				pkg.ReadInt();
				pkg.ReadBoolean();
				pkg.ReadInt();
				pkg.ReadString();
				pkg.ReadInt();
				pkg.ReadInt();
				pkg.ReadInt();
				pkg.ReadInt();
				pkg.ReadInt();
				pkg.ReadInt();
				pkg.ReadInt();
				pkg.ReadInt();
				pkg.ReadString();
				pkg.ReadInt();
				pkg.ReadString();
				pkg.ReadInt();
				pkg.ReadBoolean();
				pkg.ReadInt();
				if (pkg.ReadBoolean())
				{
					pkg.ReadInt();
					pkg.ReadString();
				}
				pkg.ReadInt();
				pkg.ReadInt();
				pkg.ReadInt();
				pkg.ReadInt();
				pkg.ReadInt();
				pkg.ReadInt();
				int num4 = pkg.ReadInt();
				int num5 = pkg.ReadInt();
				pkg.ReadInt();
				if (GamePlayerId == num5)
				{
					Team = num4;
				}
				int num6 = pkg.ReadInt();
				for (int j = 0; j < num6; j++)
				{
					pkg.ReadInt();
					pkg.ReadInt();
					pkg.ReadInt();
					pkg.ReadString();
					pkg.ReadInt();
					pkg.ReadInt();
					int num7 = pkg.ReadInt();
					for (int l = 0; l < num7; l++)
					{
						pkg.ReadInt();
						pkg.ReadInt();
					}
				}
				int num2 = pkg.ReadInt();
				for (int k = 0; k < num2; k++)
				{
					pkg.ReadInt();
				}
			}
		}

		private void SendDirection(int Direction)
		{
			GSPacketIn pkg = new GSPacketIn(91)
			{
				Parameter1 = GamePlayerId
			};
			pkg.WriteByte(7);
			pkg.WriteInt(Direction);
			SendTCP(pkg);
		}

		public void SendGameCMDShoot(int x, int y, int force, int angle)
		{
			GSPacketIn pkg = new GSPacketIn(91)
			{
				Parameter1 = GamePlayerId
			};
			pkg.WriteByte(2);
			pkg.WriteInt(x);
			pkg.WriteInt(y);
			pkg.WriteInt(force);
			pkg.WriteInt(angle);
			SendTCP(pkg);
		}

		public void sendGameCMDStunt(int type)
		{
			GSPacketIn pkg = new GSPacketIn(91)
			{
				Parameter1 = GamePlayerId
			};
			pkg.WriteByte(15);
			pkg.WriteInt(type);
			SendTCP(pkg);
		}

		public void SendLoadingComplete(int state)
		{
			GSPacketIn pkg = new GSPacketIn(91)
			{
				Parameter1 = GamePlayerId
			};
			pkg.WriteByte(16);
			pkg.WriteInt(state);
			pkg.WriteInt(104);
			pkg.WriteInt(GamePlayerId);
			SendTCP(pkg);
		}

		private void SendSelfTurn(bool fire)
		{
			SendSelfTurn(fire, useBuff: false);
		}

		private void SendSelfTurn(bool fire, bool useBuff)
		{
			if (fire)
			{
				FindTarget();
			}
		}

		public void SendShootTag(bool b, int time)
		{
			GSPacketIn pkg = new GSPacketIn(91)
			{
				Parameter1 = GamePlayerId
			};
			pkg.WriteByte(96);
			pkg.WriteBoolean(b);
			pkg.WriteByte((byte)time);
			SendTCP(pkg);
		}

		private void SendSkipNext()
		{
			GSPacketIn pkg = new GSPacketIn(91)
			{
				Parameter1 = GamePlayerId
			};
			pkg.WriteByte(12);
			pkg.WriteByte(100);
			SendTCP(pkg);
		}

		internal void SendTCP(GSPacketIn pkg)
		{
			CurRoom.SendTCP(pkg);
		}

		public void SendUseProp(int templateId)
		{
			SendUseProp(templateId, 0, 0, 0, 0);
		}

		public void SendUseProp(int templateId, int x, int y, int force, int angle)
		{
			GSPacketIn pkg = new GSPacketIn(91)
			{
				Parameter1 = GamePlayerId
			};
			pkg.WriteByte(32);
			pkg.WriteByte(3);
			pkg.WriteInt(-1);
			pkg.WriteInt(templateId);
			SendTCP(pkg);
			if (templateId == 10001 || templateId == 10002)
			{
				ItemTemplateInfo info = ItemMgr.FindItemTemplate(templateId);
				for (int i = 0; i < info.Property2; i++)
				{
					SendGameCMDShoot(x, y, force, angle);
				}
			}
		}

		public void Update(long tick)
		{
			if (m_passTick >= tick)
			{
				return;
			}
			ArrayList list;
			lock (m_actions)
			{
				list = (ArrayList)m_actions.Clone();
				m_actions.Clear();
			}
			if (list == null || seflRoom == null || list.Count <= 0)
			{
				return;
			}
			ArrayList list2 = new ArrayList();
			foreach (IAction action in list)
			{
				try
				{
					action.Execute(this, tick);
					if (!action.IsFinished(tick))
					{
						list2.Add(action);
					}
				}
				catch (Exception exception)
				{
					log.Error("Bot action error:", exception);
				}
			}
			AddAction(list2);
		}
	}
}
