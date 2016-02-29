using System;
using System.Xml.Serialization;

namespace GameCompletionHelper.Model
{
    [Serializable]
    public class Game : IGame
    {
        public string PathToExe { get; set; }
        [XmlIgnore]
        public TimeSpan TimePlayed { get; set; }
        public DateTime LastLaunched { get; set; }
        public string Name { get; set; }
        [XmlElement("TimePlayedTicks")]
        public long TimePlayedTicks
        {
            get { return TimePlayed.Ticks; }
            set { TimePlayed = new TimeSpan(value); }
        }
        public override string ToString()
        {
            return this.Name;
        }
    }
}
