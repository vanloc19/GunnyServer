namespace Game.Server
{
    public class ActiveStatusInfo
    {
        public eActive Id { get; set; }

        public int OldState { get; set; }

        public int State { get; set; }

        public bool OnChange()
        {
            return OldState != State;
        }

        public void OnSuccess(int value)
        {
            OldState = value;
        }

        public void OnUpdate()
        {
            State = OldState == 1 ? 0 : 1;
        }

        public void OnOpen()
        {
            OldState = 0;
            State = 1;
        }

        public void OnClose()
        {
            OldState = 0;
            State = 0;
        }
    }
}