using System;
using com.homemade.mec;

namespace com.homemade.tick
{
    public static class TimerExtension
    {
        public static void Register(this TickData data)
        {
            TimerManager.Instance?.Tick.Register(data);
        }

        public static void UnRegister(this TickData data)
        {
            TimerManager.Instance?.Tick.UnRegister(data);

            data.IsRunning = false;
        }

        public static bool IsValid(this TickData data)
        {
            if (TimerManager.Instance)
                return TimerManager.Instance.Tick.IsValid(data);
            return false;
        }

        public static void Register(this InvokeKey key, Action action, float delay = -1,
            Segment segment = Segment.Update)
        {
            if (delay > 172800)
                return;

            if (delay.Equals(-1))
                action.Invoke();
            else
            {
                key.UnRegister();
                TimerManager.Instance?.Invoke.Register(key, action, delay, segment);
            }
        }

        public static void Register(this InvokeKey key, Action action, int frame = 0, Segment segment = Segment.Update)
        {
            if (frame == 0)
                action.Invoke();
            else
            {
                key.UnRegister();
                TimerManager.Instance?.Invoke.Register(key, action, frame, segment);
            }
        }

        public static void UnRegister(this InvokeKey key)
        {
            if (key == null)
                return;

            if (!key.IsRunning)
                return;

            TimerManager.Instance?.Invoke.UnRegister(key);

            key.Key = -1;
        }
    }
}