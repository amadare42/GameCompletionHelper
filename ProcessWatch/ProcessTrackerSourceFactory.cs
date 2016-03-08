using ProcessWatch.Interfaces;

namespace ProcessWatch
{
    public class ProcessTrackerSourceFactory
    {
        static private DefaultProcessProvider processProvider;
        static private ProcessHook processNotifier;
        static private ActiveWindowHook activeWindowNotifier;

        static private ProcessTrackerSourceFactory defaultFactory;

        private ProcessTrackerSourceFactory()
        {
            processProvider = new DefaultProcessProvider();
            processNotifier = new ProcessHook();
            activeWindowNotifier = new ActiveWindowHook();
        }

        public static IProgramTrackerSource GetProcessTrackerSource()
        {
            if (defaultFactory == null)
            {
                defaultFactory = new ProcessTrackerSourceFactory();
            }
            return defaultFactory.InternalGetProcessTrackerSource();
        }

        private IProgramTrackerSource InternalGetProcessTrackerSource()
        {
            return new ProcessTrackerSource<ProcessHook, ActiveWindowHook>(processProvider, processNotifier, activeWindowNotifier);
        }
    }
}