using System;
using System.Configuration;
using System.Reflection;
using System.Threading;
using Game.Base.Events;
using Game.Server;
using Game.Server.Packets;
using Game.Server.Packets.Client;
using log4net;

namespace Game.Base.Packets
{
	public class PacketProcessor
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		protected IPacketHandler m_activePacketHandler;

		protected GameClient m_client;

		protected int m_handlerThreadID;

		protected static readonly IPacketHandler[] m_packetHandlers = new IPacketHandler[512];


		public PacketProcessor(GameClient client)
		{
			m_client = client;
		}

		public void HandlePacket(GSPacketIn packet)
		{
			int code = packet.Code;
			Statistics.BytesIn += packet.Length;
			Statistics.PacketsIn++;
			IPacketHandler handler = null;
			if (code < m_packetHandlers.Length)
			{
				handler = m_packetHandlers[code];
				if (handler == null)
				{
					return;
				}
				try
				{
					handler.ToString();
					if (Convert.ToBoolean(ConfigurationManager.AppSettings["OpenPacketHandler"]))
                    {
						try
                        {
							handler = m_packetHandlers[code];
							log.FatalFormat("{0}::{1}|{2}", handler.GetType().Name, (ePackageType)code, code);
						}
						catch
                        {
							log.ErrorFormat("HandlePacket(null)::{0}|{1}", (ePackageType)code, code);
						}
                    }
				}
				catch
				{
					Console.WriteLine("______________ERROR______________");
					Console.WriteLine("___ Received code: " + code + " <" + $"0x{code:x}" + "> ____");
					Console.WriteLine("_________________________________");
				}
			}
			else if (log.IsErrorEnabled)
			{
				log.ErrorFormat("Received packet code is outside of m_packetHandlers array bounds! " + m_client.ToString());
				log.Error(Marshal.ToHexDump(string.Format("===> <{2}> Packet 0x{0:X2} (0x{1:X2}) length: {3} (ThreadId={4})", code, code ^ 0xA8, m_client.TcpEndpoint, packet.Length, Thread.CurrentThread.ManagedThreadId), packet.Buffer));
			}
			if (handler == null)
			{
				return;
			}
			long tickCount = Environment.TickCount;
			try
			{
				if (m_client != null && packet != null && m_client.TcpEndpoint != "not connected")
				{
					handler.HandlePacket(m_client, packet);
				}
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					string tcpEndpoint = m_client.TcpEndpoint;
					log.Error("Error while processing packet (handler=" + handler.GetType().FullName + "  client: " + tcpEndpoint + ")", exception);
					log.Error(Marshal.ToHexDump("Package Buffer:", packet.Buffer, 0, packet.Length));
				}
			}
			long num3 = Environment.TickCount - tickCount;
			m_activePacketHandler = null;
			if (log.IsDebugEnabled)
			{
				log.Debug("Package process Time:" + num3 + "ms!");
			}
			if (num3 > 1500)
			{
				string str2 = m_client.TcpEndpoint;
				if (log.IsWarnEnabled)
				{
					log.Warn(string.Concat("(", str2, ") Handle packet Thread ", Thread.CurrentThread.ManagedThreadId, " ", handler, " took ", num3, "ms!"));
				}
			}
		}

		[ScriptLoadedEvent]
		public static void OnScriptCompiled(RoadEvent ev, object sender, EventArgs args)
		{
			Array.Clear(m_packetHandlers, 0, m_packetHandlers.Length);
			int num = SearchPacketHandlers("v168", Assembly.GetAssembly(typeof(GameServer)));
			if (log.IsInfoEnabled)
			{
				log.Info("PacketProcessor: Loaded " + num + " handlers from GameServer Assembly!");
			}
		}

		public static void RegisterPacketHandler(int packetCode, IPacketHandler handler)
		{
			m_packetHandlers[packetCode] = handler;
		}

		protected static int SearchPacketHandlers(string version, Assembly assembly)
		{
			int num = 0;
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				if (type.IsClass && type.GetInterface("Game.Server.Packets.Client.IPacketHandler") != null)
				{
					PacketHandlerAttribute[] customAttributes = (PacketHandlerAttribute[])type.GetCustomAttributes(typeof(PacketHandlerAttribute), inherit: true);
					if (customAttributes.Length != 0)
					{
						num++;
						RegisterPacketHandler(customAttributes[0].Code, (IPacketHandler)Activator.CreateInstance(type));
					}
				}
			}
			return num;
		}
	}
}
