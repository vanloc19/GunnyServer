using System;
using System.Drawing;

namespace Game.Logic.Phy.Object
{
	public class Ball : PhysicalObj
	{

		public override int Type => 0;

		public Ball(int id, string name, string defaultAction, int scale, int rotation) : base(id, name, "asset.game.six.ball", defaultAction, scale, rotation, 0)
		{
			m_rect = new Rectangle(-30, -30, 60, 60);
			base.CanPenetrate = true;
		}

		public override void CollidedByObject(Physics phy)
		{
			if (phy is SimpleBomb)
			{
				(phy as SimpleBomb).Owner.PickPhy(this);
			}
		}
	}
}
