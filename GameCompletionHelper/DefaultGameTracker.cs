using System.Collections.Generic;
using ProcessWatch;
using System.Linq;

namespace GameCompletionHelper
{
    class DefaultGameTracker : IGameTracker
    {
        public ProcessTracker tracker;

        public DefaultGameTracker()
        {
            tracker = new ProcessTracker();
        }

        public void AddGame(ITrackableProgram game)
        {
            tracker.TrackedPrograms.Add(game);
            tracker.CheckRunningProcesses();
        }

        public void RemoveGame(ITrackableProgram game)
        {
            tracker.TrackedPrograms.Remove(game);
        }

        public void SetGames(IEnumerable<ITrackableProgram> games)
        {
            tracker.TrackedPrograms = games.ToList();
            tracker.CheckRunningProcesses();
        }
    }
}
