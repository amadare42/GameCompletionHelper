using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace GameCompletionHelper.Model
{
    [Serializable]
    public class Game : IGame
    {
        public Game()
        {
            this.Sessions = new List<GameSession>();
        }

        public string PathToExe { get; set; }

        [XmlIgnore]
        public TimeSpan PlayedTotal
        {
            get
            {
                TimeSpan total = TimeSpan.Zero;
                Sessions.ForEach(session => total += session.TimePlayed);
                return total;
            }
        }

        public List<GameSession> Sessions { get; set; }

        IEnumerable<GameSession> IGame.Sessions
        {
            get
            {
                return this.Sessions;
            }
        }

        public DateTime LastLaunched
        {
            get
            {
                if (Sessions.Count == 0)
                    return default(DateTime);
                return Sessions.Max(s => s.SessionStart);
            }
        }

        public string Name { get; set; }

        public override string ToString()
        {
            return this.Name;
        }

        public void AddSession(GameSession session)
        {
            this.Sessions.Add(session);
        }

        public void RemoveSession(GameSession session)
        {
            this.Sessions.Remove(session);
        }

        public bool HasSessionAt(DateTime startTime)
        {
            return this.Sessions.Any(s => s.SessionStart == startTime);
        }

        public GameSession GetSessionAt(DateTime startTime)
        {
            return this.Sessions.FirstOrDefault(s => s.SessionStart == startTime);
        }

        public static Game Empty
        {
            get
            {
                return new Game
                {
                    Name = "New Game"
                };
            }
        }

        public bool RunAsAdmin { get; set; }

        public string RunPath
        {
            get; set;
        }
    }
}