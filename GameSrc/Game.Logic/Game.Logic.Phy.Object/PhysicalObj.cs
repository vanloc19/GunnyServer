using System.Collections.Generic;
using Game.Logic.Actions;

namespace Game.Logic.Phy.Object
{
	public class PhysicalObj : Physics
	{
		private Dictionary<string, string> m_actionMapping;

		private string m_model;

		private string m_currentAction;

		private int m_scale;

		private int m_rotation;

		private BaseGame m_game;

		private bool m_canPenetrate;

		private string m_name;

		private int m_phyBringToFront;

		private int m_type;

		private int m_typeEffect;

		public virtual int Type
		{
			get
			{
				return m_type;
			}
			set
			{
				m_type = value;
			}
		}

		public string Model => m_model;

		public string CurrentAction
		{
			get
			{
				return m_currentAction;
			}
			set
			{
				m_currentAction = value;
			}
		}

		public int Scale => m_scale;

		public int Rotation => m_rotation;

		public virtual int phyBringToFront => m_phyBringToFront;

		public int typeEffect => m_typeEffect;

		public bool CanPenetrate
		{
			get
			{
				return m_canPenetrate;
			}
			set
			{
				m_canPenetrate = value;
			}
		}

		public string Name => m_name;

		public Dictionary<string, string> ActionMapping => m_actionMapping;

		public void SetGame(BaseGame game)
		{
			m_game = game;
		}

		public void PlayMovie(string action, int delay, int movieTime)
		{
			if (m_game != null)
			{
				m_game.AddAction(new PhysicalObjDoAction(this, action, delay, movieTime));
			}
		}

		public override void CollidedByObject(Physics phy)
		{
			if (!m_canPenetrate && phy is SimpleBomb)
			{
				((SimpleBomb)phy).Bomb();
			}
		}

		public PhysicalObj(int id, string name, string model, string defaultAction, int scale, int rotation, int typeEffect) : base(id)
		{
			m_name = name;
			m_model = model;
			m_currentAction = defaultAction;
			m_scale = scale;
			m_rotation = rotation;
			m_canPenetrate = false;
			m_typeEffect = typeEffect;
			switch (name)
			{
				case "hide":
					m_phyBringToFront = 6;
					break;
				case "top":
					m_phyBringToFront = 1;
					break;
				case "normal":
					m_phyBringToFront = 0;
					break;
				default:
					m_phyBringToFront = -1;
					break;
			}
		}

		public PhysicalObj(int id, string name, string model, string defaultAction, int scale, int rotation, int typeEffect, bool canPenetrate) : base(id)
        {
			m_name = name;
			m_model = model;
			m_currentAction = defaultAction;
			m_scale = scale;
			m_rotation = rotation;
			m_canPenetrate = canPenetrate;
			m_typeEffect = typeEffect;

			if (name == "hide")
				m_phyBringToFront = 6;
			else if (name == "top")
				m_phyBringToFront = 1;
			else if (name == "normal")
				m_phyBringToFront = 0;
			else
				m_phyBringToFront = -1;
        }
	}
}
