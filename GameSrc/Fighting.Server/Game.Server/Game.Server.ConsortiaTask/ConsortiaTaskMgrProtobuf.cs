using System.Collections.Generic;
using Game.Server.ConsortiaTask.Data;
using ProtoBuf;

namespace Game.Server.ConsortiaTask
{
	[ProtoContract]
	public class ConsortiaTaskMgrProtobuf
	{
		[ProtoMember(1)]
		public List<ConsortiaTaskUserTempInfo> tempUsers
		{
			get;
			set;
		}
	}
}
