namespace CodenjoyBot.Board
{
    public class Frame
    {
        public CodenjoyBot.Board.Board Board { get; }
        public uint Time { get; }

        public Frame(CodenjoyBot.Board.Board board, uint time)
        {
            Board = board;
            Time = time;
        }
    }
}