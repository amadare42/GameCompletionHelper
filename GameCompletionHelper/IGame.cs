using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCompletionHelper
{
    public interface IGame
    {
        string PathToExe { get; set; }
        TimeSpan TimePlayed { get; set; }
        DateTime LastLaunched { get; set; }
        string Name { get; set; }
    }
}
