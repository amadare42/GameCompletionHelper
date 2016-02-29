using System;
using System.Xml.Serialization;

namespace GameCompletionHelper.Model
{
    public class GameSession
    {
        [XmlIgnore]
        public TimeSpan TimePlayed { get; set; }
        public DateTime SessionStart { get; set; }

        [XmlElement("TimePlayedTicks")]
        public long TimePlayedTicks
        {
            get { return TimePlayed.Ticks; }
            set { TimePlayed = new TimeSpan(value); }
        }
    }
}
