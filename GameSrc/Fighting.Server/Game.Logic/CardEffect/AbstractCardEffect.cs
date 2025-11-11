using System;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using Game.Logic.CardEffect;

namespace Game.Logic.CardEffects
{
    public abstract class AbstractCardEffect
    {
        private eCardEffectType m_type;
        protected Living m_living;
        public Random rand;
        public bool IsTrigger;        
        private CardBuffInfo m_buffInfo;
        public AbstractCardEffect(eCardEffectType type, CardBuffInfo buff)
        {
            rand = new Random();
            m_type = type;
            m_buffInfo = buff;            
        }       

        public CardBuffInfo BuffInfo
        {
            get { return m_buffInfo; }
        }

        public eCardEffectType Type
        {
            get { return m_type; }
        }

        public int TypeValue
        {
            get { return (int)m_type; }
        }

        public virtual bool Start(Living living)
        {
            m_living = living;
            if (m_living.CardEffectList.Add(this))
            {
                return true;
            }
            return false;
        }

        public virtual bool Stop()
        {
            if (m_living != null)
            {
                return m_living.CardEffectList.Remove(this);
            }
            return false;
        }

        public virtual bool Pause()
        {
            if (m_living != null)
            {
                return m_living.CardEffectList.Pause(this);
            }
            return false;
        }

        public virtual void OnAttached(Living living) { }
        public virtual void OnRemoved(Living living) { }
        public virtual void OnPaused(Living living) { }


    }
}
