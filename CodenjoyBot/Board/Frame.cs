namespace CodenjoyBot.Board
{
    public class Frame<T> where T: class
    {
        public T Board { get; }
        public uint Time { get; }

        public Frame(T board, uint time)
        {
            Board = board;
            Time = time;
        }
    }
}