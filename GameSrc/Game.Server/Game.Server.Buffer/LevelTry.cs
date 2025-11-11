using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Buffer
{
    public class LevelTry : AbstractBuffer
    {
        public LevelTry(BufferInfo buffer) : base(buffer)
        {

        }

        public override void Start(GamePlayer player)
        {
            LevelTry ofType = player.BufferList.GetOfType(typeof(LevelTry)) as LevelTry;
            if (ofType != null)
            {
                ofType.Info.ValidDate += base.Info.ValidDate;
                player.BufferList.UpdateBuffer(ofType);
                player.UpdateFightBuff(base.Info);
            }
            else
            {
                base.Start(player);
                player.FightBuffs.Add(base.Info);
            }
        }

        public override void Stop()
        {
            m_player.FightBuffs.Remove(base.Info);
            base.Stop();
        }
    }
}