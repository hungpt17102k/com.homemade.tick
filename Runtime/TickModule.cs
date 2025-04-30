using com.homemade.mec;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace com.homemade.tick
{
    public class TickModule : MonoBehaviour
    {
        private readonly Dictionary<TickKey, TickHandle> handle = new();

        public void Register(TickData data)
        {
            if (data.Action == null)
                return;

            UnRegister(data);

            if (!FindKey(data, out var key))
            {
                handle.Add(key, new TickHandle());
                Play(key);
            }

            handle[key].Data.Add(data);

            data.IsRunning = true;
            if (data.Delay <= 1)
            {
                try
                {
                    data.Action?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError($"[Timer]: {e.Message}");
                }
            }
        }

        private bool FindKey(TickData data, out TickKey key)
        {
            key = handle.Keys.ToList().Find(x => x.Same(data));
            if (key == null)
            {
                key = new TickKey(data);
                return false;
            }

            return true;
        }

        public void UnRegister(TickData data)
        {
            if (FindKey(data, out var key))
            {
                if (handle[key].Data.Contains(data))
                {
                    handle[key].Data.Remove(data);
                    if (handle[key].Data.Count == 0)
                    {
                        Stop(key);
                        handle.Remove(key);
                    }

                    data.IsRunning = false;
                }
            }
        }

        public bool IsValid(TickData data)
        {
            if (FindKey(data, out var key))
                return data is { Action: not null, IsRunning: true } && handle[key].Data.Contains(data);
            return false;
        }

        public void Clear()
        {
            foreach (var item in handle.Values.ToList())
            {
                foreach (var tick in item.Data.ToList())
                {
                    if (tick != null)
                        tick.IsRunning = false;
                }
            }

            handle.Clear();
        }

        private void Play(TickKey key)
        {
            handle[key].Handle = Timing.RunCoroutine(_Tick(key), key.Data.Segment);
        }

        private void Stop(TickKey key)
        {
            Timing.KillCoroutines(handle[key].Handle);
        }

        private IEnumerator<float> _Tick(TickKey key)
        {
            while (true)
            {
                if (key.Data.Delay == 0)
                    yield return Timing.WaitForOneFrame;
                else
                    yield return Timing.WaitForSeconds(key.Data.Delay);

                foreach (var data in handle[key].Data.ToList())
                {
                    if (data.IsRunning)
                        data.Action?.Invoke();
                }
            }
        }

        private class TickKey
        {
            public TickData Data = new() { Segment = Segment.Update, Delay = 0 };

            public TickKey(TickData data)
            {
                Data.Segment = data.Segment;
                Data.Delay = data.Delay;
            }

            public bool Same(TickData data)
            {
                return Data.Segment == data.Segment && Data.Delay.Equals(data.Delay);
            }
        }

        private class TickHandle
        {
            public CoroutineHandle Handle;
            public List<TickData> Data = new();
        }
    }

    public class TickData
    {
        public Segment Segment = Segment.Update;
        public float Delay;

        public Action Action = null;
        public bool IsRunning;

        public TickData(Segment segment = Segment.Update, float delay = 0f)
        {
            Segment = segment;
            Delay = delay;
        }
    }
}