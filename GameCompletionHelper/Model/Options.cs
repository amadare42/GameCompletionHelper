using System;

namespace GameCompletionHelper.Model
{
    [Serializable]
    public class Options
    {
        public string RunPath { get; set; } = string.Empty;
        public bool RunAsAdmin { get; set; }
        public bool MinimizeWindowsOnStart { get; set; }

        //todo: add to Options
        /*
            * Tags
            * calculating time only if window is active
            * working catalog
            * include to randomizator
            * desired game session length
        */
    }
}