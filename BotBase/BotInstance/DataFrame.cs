using System;

namespace BotBase
{
    public struct DataFrame
    {
        public DateTime Time { get; }

        public string Board { get; }

        public uint FrameNumber { get; }

        public DataFrame(DateTime time, string board, uint frameNumber)
        {
            Time = time;
            Board = board;
            FrameNumber = frameNumber;
        }

    }
}