using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;

namespace Game.Server.Quests
{
    public class PlayerLoginDeviceCondiction : BaseCondition
    {
        public PlayerLoginDeviceCondiction(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
        {

        }

        public override void AddTrigger(GamePlayer player)
        {
            player.LoginDeviceAdd += LoginGame;
        }

        public void LoginGame()
        {
            if (base.Value > 0)
                base.Value--;
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.LoginDeviceAdd -= LoginGame;
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return base.Value <= 0;
        }
    }
}