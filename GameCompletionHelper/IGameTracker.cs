using ProcessWatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCompletionHelper
{
    public interface IGameTracker
    {
        void AddGame(ITrackableProgram game);
        void RemoveGame(ITrackableProgram game);
        void SetGames(IEnumerable<ITrackableProgram> games);
    }
}
