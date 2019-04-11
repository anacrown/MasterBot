using System.Collections.Generic;
using System.Linq;

namespace CodenjoyBot.Board
{
    public class FrameBuffer<T> where T: class
    {
        public int MaxSize { get; }

        private readonly SortedDictionary<uint, Frame<T>> _frames = new SortedDictionary<uint, Frame<T>>();

        public FrameBuffer(int maxSize)
        {
            MaxSize = maxSize;
        }

        public void Clear() => _frames?.Clear();

        public void AddFrame(Frame<T> frame)
        {
            if (!_frames.ContainsKey(frame.Time))
                _frames.Add(frame.Time, frame);

            if (MaxSize > 0 && _frames.Count > MaxSize)
                _frames.Remove(_frames.Select(t => t.Key).Min());
        }

        public Frame<T> this[uint time] => _frames.ContainsKey(time) ? _frames[time] : null;
    }
}