using System;

namespace GameCompletionHelper.Model
{
    [Serializable]
    public class Options
    {
        public string RunPath { get; set; } = string.Empty;
        public bool RunAsAdmin { get; set; }
        public bool MinimizeWindowsOnStart { get; set; }
        public bool CalcOnlyOnActive { get; set; }

        //todo: add to Options
        /*
            * Tags
            * working catalog
            * include to randomizator
            * desired game session length
        */
    }
}