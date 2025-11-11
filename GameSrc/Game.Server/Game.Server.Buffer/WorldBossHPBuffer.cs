using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Buffer
{
    public class WorldBossHPBuffer : AbstractBuffer
    {
        public WorldBossHPBuffer(BufferInfo buffer) : base(buffer)
        {
        }

        public override void Start(GamePlayer player)
        {
            if (player.BufferList.GetOfType(typeof(WorldBossHPBuffer)) is WorldBossHPBuffer buffer)
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