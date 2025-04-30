using com.homemade.mec;
using com.homemade.pattern.singleton;

namespace com.homemade.tick
{
    public class TimerManager : LiveSingleton<TimerManager>
    {
        public TickModule Tick => tick;
        public new InvokeModule Invoke => invoke;

        private TickModule tick;
        private InvokeModule invoke;

        protected override void OnInit()
        {
            base.OnInit();

            tick = gameObject.AddComponent<TickModule>();
            invoke = gameObject.AddComponent<InvokeModule>();
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();

            Clear();
        }

        public void Clear()
        {
            Timing.KillCoroutines();
            tick.Clear();
            invoke.Clear();
        }
    }
}
