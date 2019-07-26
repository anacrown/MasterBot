namespace BotBase.BotInstance
{
    public struct DataFrame
    {

        public DataFrame(uint time, string board)
        {
            Time = time;
            Board = board;
        }

        public uint Time { get; }
        public string Board { get; }
    }
}