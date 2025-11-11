using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Buffer
{
    public class WorldBossAttrackBuffer : AbstractBuffer
    {
        public WorldBossAttrackBuffer(BufferInfo buffer) : base(buffer)
        {
        }

        public override void Start(GamePlayer player)
        {
            if (player.BufferList.GetOfType(typeof(WorldBossAttrackBuffer)) is WorldBossAttrackBuffer buffer)
            {
                buffer.Info.ValidDate = Info.ValidDate;
                player.BufferList.UpdateBuffer(buffer);
                player.UpdateFightBuff(Info);
            }
            else
            {
                base.Start(player);
                player.FightBuffs.Add(Info);
            }
        }

        public override void Stop()
        {
            m_player.FightBuffs.Remove(Info);
            base.Stop();
        }
    }
}