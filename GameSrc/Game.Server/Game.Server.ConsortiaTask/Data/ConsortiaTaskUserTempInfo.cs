using ProtoBuf;

namespace Game.Server.ConsortiaTask.Data
{
	[ProtoContract]
	public class ConsortiaTaskUserTempInfo
	{
		[ProtoMember(1)]
		public int UserID
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int Total
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int Exp
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int Offer
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int Riches
		{
			get;
			set;
		}
	}
}
