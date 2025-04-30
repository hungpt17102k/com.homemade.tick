using com.homemade.mec;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.homemade.tick
{
    public class InvokeModule : MonoBehaviour
    {
        private readonly Dictionary<InvokeKey, CoroutineHandle> invokes = new();
        private int idxInvoke;

        public void Clear()
        {
            foreach (var invoke in invokes.Keys.ToList())
            {
                if (invoke != null)
                    invoke.Key = -1;
            }

            invokes.Clear();
            idxInvoke = 0;
        }

        public InvokeKey Register(InvokeKey key, Action action, int frame, Segment segment = Segment.Update)
        {
            key.Key = idxInvoke;
            var handle = Timing.RunCoroutine(_Invoke(key, action, frame), segment);
            invokes.Add(key, handle);
            idxInvoke++;

            return key;
        }

        public InvokeKey Register(InvokeKey key, Action action, float delay, Segment segment = Segment.Update)
        {
            key.Key = idxInvoke;
            var handle = Timing.RunCoroutine(_Invoke(key, action, delay), segment);
            invokes.Add(key, handle);
            idxInvoke++;

            return key;
        }

        private IEnumerator<float> _Invoke(InvokeKey key, Action action, float delay)
        {
            yield return Timing.WaitForSeconds(delay);

            Invoke(key, action);
        }

        private IEnumerator<float> _Invoke(InvokeKey key, Action action, int frame)
        {
            for (int i = 0; i < frame; i++)
                yield return Timing.WaitForOneFrame;

            Invoke(key, action);
        }

        private void Invoke(InvokeKey key, Action action)
        {
            UnRegister(key);

            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"[Timer]: {e.Message}");
            }
        }

        public void UnRegister(InvokeKey key)
        {
            if (!invokes.ContainsKey(key))
                return;

            Timing.KillCoroutines(invokes[key]);
            invokes[key] = default;

            invokes.Remove(key);
            key.Key = -1;
        }
    }

    public class InvokeKey
    {
        public bool IsRunning => Key != -1;
        public int Key = -1;
    }
}