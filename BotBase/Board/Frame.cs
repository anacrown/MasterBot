using System;

namespace BotBase.Board
{
    public class Frame<T> where T: class
    {
        public T Board { get; }
        public uint FrameNumber { get; }

        public DateTime Time { get; set; }

        public Frame(DateTime time, T board, uint frameNumber)
        {
            Time = time;
            Board = board;
            FrameNumber = frameNumber;
        }
    }
}