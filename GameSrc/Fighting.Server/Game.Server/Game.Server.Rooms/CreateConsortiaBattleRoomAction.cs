using System.Linq;
using Bussiness;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Games;

namespace Game.Server.Rooms
{
    public class CreateConsortiaBattleRoomAction : IAction
    {
        private readonly GamePlayer _player;

        private readonly string _name;

        private readonly string _password;

        private readonly eRoomType _roomType;

        private readonly byte _timeType;

        public CreateConsortiaBattleRoomAction(GamePlayer player)
        {
            _player = player;
            _name = "Consortia Battle";
            _password = "12dasSda44";
            _roomType = eRoomType.ConsortiaBattle;
            _timeType = 2;
        }

        public void Execute()
        {
            _player.CurrentRoom?.RemovePlayerUnsafe(_player);

            if (_player.IsActive == false)
                return;

            var rooms = RoomMgr.Rooms;
            var room = rooms.FirstOrDefault(t => !t.IsUsing);

            if (room != null && GameMgr.GuildBattle.CanEnterGame(_player.PlayerCharacter.ConsortiaID))
            {
                RoomMgr.WaitingRoom.RemovePlayer(_player);
                room.Start();
                room.UpdateRoom(_name, _password, _roomType, _timeType, 0);
                room.AreaID = _player.ZoneId;
                room.isCrosszone = false;
                _player.Out.SendSingleRoomCreate(room);
                room.AddPlayerUnsafe(_player);
            }
            else
            {
                _player.SendMessage(LanguageMgr.GetTranslation("GameServer.GuildBattle.EnterGame.Error"));
            }
        }
    }
}