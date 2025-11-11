//using Game.Logic.Phy.Object;

//namespace Game.Logic.Actions
//{
//	public class LivingRotateTurnAction : BaseAction
//	{
//		private string m_endPlay;

//		private Living m_living;

//		private int m_rotation;

//		private int m_speed;

//		public LivingRotateTurnAction(Player player, int rotation, int speed, string endPlay, int delay)
//			: base(0, delay)
//		{
//			m_player = player;
//			m_rotation = rotation;
//			m_speed = speed;
//			m_endPlay = endPlay;
//		}

//		protected override void ExecuteImp(BaseGame game, long tick)
//		{
//			game.SendLivingTurnRotation(m_player, m_rotation, m_speed, m_endPlay);
//			Finish(tick);
//		}
//	}
//}
using Game.Logic.Phy.Object;

namespace Game.Logic.Actions
{
    public class LivingRotateTurnAction : BaseAction
    {
        private Living m_living;

        private int m_speed;

        private int m_angle;

        private string m_endPlay;

        public LivingRotateTurnAction(Living living, int angle, int speed, string endPlay, int delay) : base(0, delay)
        {
            m_living = living;
            m_speed = speed;
            m_angle = angle;
            m_endPlay = endPlay;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            game.LivingChangeAngle(m_living, m_speed, m_angle, m_endPlay);
            base.Finish(tick);
        }
    }
}
